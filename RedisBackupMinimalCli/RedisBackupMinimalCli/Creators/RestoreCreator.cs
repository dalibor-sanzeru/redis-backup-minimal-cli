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

            for (int i = 0; i < commands.Count; i++)
            {
                string command = commands[i];
                var commandType = this.redisTypeDeSerializer.GetRedisType(command);

                switch (commandType)
                {
                    case RedisType.String:
                        break;
                    case RedisType.List:
                        break;
                    case RedisType.Set:
                        break;
                    case RedisType.SortedSet:
                        break;
                    case RedisType.Hash:
                        break;
                    case RedisType.Stream:
                        break;
                    case RedisType.None:
                    case RedisType.Unknown:
                    default:
                        throw new InvalidOperationException($"Type {commandType} cannot be processed.");
                }
            }

            throw new NotImplementedException();
        }
    }
}
