using RedisBackupMinimalCli.PersistanceOperations;
using RedisBackupMinimalCli.Serialization;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisBackupMinimalCli.Creators
{
    public class RestoreCreator : CreatorBase
    {
        private readonly ICommandPersistanceHandler commandPersistanceHandler;
        private readonly IRedisTypeDeserializer redisTypeDeSerializer;

        public RestoreCreator(IServer server, IDatabase database, IRedisTypeDeserializer redisTypeDeSerializer, ICommandPersistanceHandler commandPersistanceHandler) : base(server, database)
        {
            this.commandPersistanceHandler = commandPersistanceHandler;
            this.redisTypeDeSerializer = redisTypeDeSerializer;
        }

        public override async Task Execute(Options options)
        {
            var commands = await this.commandPersistanceHandler.LoadCommands(options.FileName);
            var batch = database.CreateBatch();

            List<(string command, string key, Task<bool> dbOperation)> commandKeysMigrationResults = new();
            for (int i = 0; i < commands.Count; i++)
            {
                string command = commands[i];
                var commandType = this.redisTypeDeSerializer.GetRedisType(command);

                switch (commandType)
                {
                    case RedisType.String:
                        var resultKey = this.redisTypeDeSerializer.DeSerializeString(command);
                        commandKeysMigrationResults.Add((RedisTypeDeserializer.KeyTypeRedisCommand, resultKey.Key, this.database.SetAddAsync(resultKey.Key, resultKey.Value)));
                        break;
                    case RedisType.List:
                        var resultList = this.redisTypeDeSerializer.DeSerializeList(command);
                        commandKeysMigrationResults.Add((RedisTypeDeserializer.ListTypeRedisCommand, resultList.Key, this.database.ListRightPushAsync(resultList.Key, resultList.Value).ContinueWith(res => res.Result > 0)));
                        break;
                    case RedisType.Set:
                        var resultSet = this.redisTypeDeSerializer.DeSerializeSet(command);
                        commandKeysMigrationResults.Add((RedisTypeDeserializer.SetTypeRedisCommand, resultSet.Key, this.database.SetAddAsync(resultSet.Key, resultSet.Value)));
                        break;
                    case RedisType.SortedSet:
                        var resultSortedSet = this.redisTypeDeSerializer.DeSerializeSortedSet(command);
                        commandKeysMigrationResults.Add((RedisTypeDeserializer.SortedSetRedisCommand, resultSortedSet.Key, this.database.SortedSetAddAsync(resultSortedSet.Key, resultSortedSet.Value.Element, resultSortedSet.Value.Score)));
                        break;
                    case RedisType.Hash:
                        var resultHash = this.redisTypeDeSerializer.DeSerializeHash(command);
                        commandKeysMigrationResults.Add((RedisTypeDeserializer.HashTypeRedisCommand, resultHash.Key, this.database.HashSetAsync(resultHash.Key, resultHash.Value.Name, resultHash.Value.Value)));
                        break;
                    case RedisType.Stream:
                        //var resultStream = this.redisTypeDeSerializer.DeSerializeStream(command);
                        //commandKeysMigrationResults.Add((RedisTypeDeserializer.HashTypeRedisCommand, resultStream.Key, this.database.StreamAddAsync(resultStream.Key, resultStream.Value.Values)));
                        break;
                    case RedisType.None:
                    case RedisType.Unknown:
                    default:
                        throw new InvalidOperationException($"Type {commandType} cannot be processed.");
                }
            }

            batch.Execute();
            await Task.WhenAll(commandKeysMigrationResults.Select(x => x.dbOperation).ToList());
        }
    }
}
