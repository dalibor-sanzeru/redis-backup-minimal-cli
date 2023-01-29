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
        public BackupCreator(IServer server, IDatabase database) : base(server, database)
        {
        }

        public override async Task Execute(Options options)
        {
            var redisTypeKeys = this.LoadKeysInBatchMode(options.Keys);

            redisTypeKeys.Result.ForEach(async keysPerType =>
            {
                switch (keysPerType.Type)
                {
                    case RedisType.String:
                        var stringResults = await ExtractInBatchMode((batch) => keysPerType.Keys.Select(individualKey => new KeyValuePair<string, Task<RedisValue>>(individualKey, batch.StringGetAsync(individualKey))));
                        var s = new StringsSerializer();
                        var r = s.Serialize(stringResults);
                        break;
                    case RedisType.List:
                        var listResults = await ExtractInBatchMode((batch) => keysPerType.Keys.Select(individualKey => new KeyValuePair<string, Task<RedisValue[]>>(individualKey, batch.ListRangeAsync(individualKey))));
                        var ss = new ListsSerializer();
                        var re = ss.Serialize(listResults);
                        break;
                    case RedisType.Set:
                        var setResults = await ExtractInBatchMode((batch) => keysPerType.Keys.Select(individualKey => new KeyValuePair<string, Task<RedisValue[]>>(individualKey, batch.SetMembersAsync(individualKey))));
                        break;
                    case RedisType.SortedSet:
                        var sortedSetResults = await ExtractInBatchMode((batch) => keysPerType.Keys.Select(individualKey => new KeyValuePair<string, Task<SortedSetEntry[]>>(individualKey, batch.SortedSetRangeByRankWithScoresAsync(individualKey))));
                        break;
                    case RedisType.Hash:
                        var hashResults = await ExtractInBatchMode((batch) => keysPerType.Keys.Select(individualKey => new KeyValuePair<string, Task<HashEntry[]>>(individualKey, batch.HashGetAllAsync(individualKey))));
                        break;
                    case RedisType.Stream:
                        var streamResults = await ExtractInBatchMode((batch) => keysPerType.Keys.Select(individualKey => new KeyValuePair<string, Task<StreamEntry[]>>(individualKey, batch.StreamRangeAsync(individualKey))));
                        break;
                    case RedisType.None:
                    case RedisType.Unknown:
                    default:
                        throw new InvalidOperationException($"Type {keysPerType.Type} cannot be processed.");
                }
            });
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

        private async Task<List<KeyValuePair<string, T>>> ExtractInBatchMode<T>(Func<IBatch, IEnumerable<KeyValuePair<string, Task<T>>>> dbExtractor)
        {
            var batch = database.CreateBatch();

            var results = dbExtractor(batch);


            batch.Execute();

            await Task.WhenAll(results.Select(x => x.Value));

            return results.Select(x => new KeyValuePair<string, T>(x.Key, x.Value.Result)).ToList();
        }
    }
}
