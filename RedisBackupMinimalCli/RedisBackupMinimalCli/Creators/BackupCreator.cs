using RedisBackupMinimalCli.FileSystemOperations;
using RedisBackupMinimalCli.Serialization;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RedisBackupMinimalCli.Creators
{
    public class BackupCreator : CreatorBase
    {
        private readonly IBackupSaver backupSaver;
        private readonly IRedisTypeSerializer redisTypeSerializer;

        public BackupCreator(IServer server, IDatabase database, IRedisTypeSerializer redisTypeSerializer, IBackupSaver backupSaver) : base(server, database)
        {
            this.backupSaver = backupSaver;
            this.redisTypeSerializer = redisTypeSerializer;
        }

        public override async Task Execute(Options options)
        {
            var redisTypeKeys = await this.LoadKeysInBatchMode(options.Keys);

            foreach (var keysPerType in redisTypeKeys)
            {
                List<string> serializedCommads = null;
                switch (keysPerType.Type)
                {
                    case RedisType.String:
                        var stringResults = await LoadAndExtract(keysPerType.Keys, (batch, individualKey) => batch.StringGetAsync(individualKey));
                        serializedCommads = this.redisTypeSerializer.SerializeStrings(stringResults);
                        break;
                    case RedisType.List:
                        var listResults = await LoadAndExtract(keysPerType.Keys, (batch, individualKey) => batch.ListRangeAsync(individualKey));
                        serializedCommads = this.redisTypeSerializer.SerializeLists(listResults);
                        break;
                    case RedisType.Set:
                        var setResults = await LoadAndExtract(keysPerType.Keys, (batch, individualKey) => batch.SetMembersAsync(individualKey));
                        serializedCommads = this.redisTypeSerializer.SerializeSets(setResults);
                        break;
                    case RedisType.SortedSet:
                        var sortedSetResults = await LoadAndExtract(keysPerType.Keys, (batch, individualKey) => batch.SortedSetRangeByRankWithScoresAsync(individualKey));
                        serializedCommads = this.redisTypeSerializer.SerializeSortedSets(sortedSetResults);
                        break;
                    case RedisType.Hash:
                        var hashResults = await LoadAndExtract(keysPerType.Keys, (batch, individualKey) => batch.HashGetAllAsync(individualKey));
                        serializedCommads =  this.redisTypeSerializer.SerializeHashSets(hashResults);
                        break;
                    case RedisType.Stream:
                        var streamResults = await LoadAndExtract(keysPerType.Keys, (batch, individualKey) => batch.StreamRangeAsync(individualKey));
                        serializedCommads = this.redisTypeSerializer.SerializeStreams(streamResults);
                        break;
                    case RedisType.None:
                    case RedisType.Unknown:
                    default:
                        throw new InvalidOperationException($"Type {keysPerType.Type} cannot be processed.");
                }
                await backupSaver.Save(options.Directory, serializedCommads);
            }
        }

        private async Task<List<(RedisType Type, List<string> Keys)>> LoadKeysInBatchMode(IEnumerable<string> keyPatterns)
        {
            var batch = database.CreateBatch();
            var allKeys = keyPatterns.SelectMany(k => server.Keys(pattern: k));
            var keysWithTypes = allKeys.ToDictionary(k => k.ToString(), k => batch.KeyTypeAsync(k));
            batch.Execute();

            await Task.WhenAll(keysWithTypes.Values);
            var redisTypeKeys = keysWithTypes
                                 .GroupBy(x => x.Value.Result)
                                 .Select(x => (type: x.Key, keys: x.Select(x => x.Key).ToList()))
                                 .ToList();

            return redisTypeKeys;
        }

        private async Task<List<KeyValuePair<string, T>>> LoadAndExtract<T>(List<string> keysPerType, Func<IBatch, string, Task<T>> dbExtractor)
        {
            var batch = database.CreateBatch();
            var results = keysPerType.Select(individualKey => new KeyValuePair<string, Task<T>>(individualKey, dbExtractor(batch, individualKey))).ToList();
            batch.Execute();

            await Task.WhenAll(results.Select(x => x.Value));

            return results.Select(x => new KeyValuePair<string, T>(x.Key, x.Value.Result)).ToList();
        }
    }
}
