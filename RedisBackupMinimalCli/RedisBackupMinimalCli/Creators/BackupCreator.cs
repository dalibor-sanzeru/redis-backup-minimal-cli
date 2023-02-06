using RedisBackupMinimalCli.PersistanceOperations;
using RedisBackupMinimalCli.Serialization;
using StackExchange.Redis;

namespace RedisBackupMinimalCli.Creators
{
    public class BackupCreator : CreatorBase
    {
        private readonly ICommandPersistanceHandler backupSaver;
        private readonly IRedisTypeSerializer redisTypeSerializer;

        public BackupCreator(IServer server, IDatabase database, IRedisTypeSerializer redisTypeSerializer, ICommandPersistanceHandler backupSaver) : base(server, database)
        {
            this.backupSaver = backupSaver;
            this.redisTypeSerializer = redisTypeSerializer;
        }

        public override async Task Execute(Options options)
        {
            var redisTypeKeys = await this.LoadKeysInBatchMode(options.Keys);

            List<string> serializedCommads = new();
            foreach (var keysPerType in redisTypeKeys)
            {
                switch (keysPerType.Type)
                {
                    case RedisType.String:
                        var stringResults = await LoadAndExtract(keysPerType.Keys, (batch, individualKey) => batch.StringGetAsync(individualKey));
                        serializedCommads.AddRange(this.redisTypeSerializer.SerializeStrings(stringResults));
                        break;
                    case RedisType.List:
                        var listResults = await LoadAndExtract(keysPerType.Keys, (batch, individualKey) => batch.ListRangeAsync(individualKey));
                        serializedCommads.AddRange(this.redisTypeSerializer.SerializeLists(listResults));
                        break;
                    case RedisType.Set:
                        var setResults = await LoadAndExtract(keysPerType.Keys, (batch, individualKey) => batch.SetMembersAsync(individualKey));
                        serializedCommads.AddRange(this.redisTypeSerializer.SerializeSets(setResults));
                        break;
                    case RedisType.SortedSet:
                        var sortedSetResults = await LoadAndExtract(keysPerType.Keys, (batch, individualKey) => batch.SortedSetRangeByRankWithScoresAsync(individualKey));
                        serializedCommads.AddRange(this.redisTypeSerializer.SerializeSortedSets(sortedSetResults));
                        break;
                    case RedisType.Hash:
                        var hashResults = await LoadAndExtract(keysPerType.Keys, (batch, individualKey) => batch.HashGetAllAsync(individualKey));
                        serializedCommads.AddRange(this.redisTypeSerializer.SerializeHashSets(hashResults));
                        break;
                    case RedisType.Stream:
                        var streamResults = await LoadAndExtract(keysPerType.Keys, (batch, individualKey) => batch.StreamRangeAsync(individualKey));
                        serializedCommads.AddRange(this.redisTypeSerializer.SerializeStreams(streamResults));
                        break;
                    case RedisType.None:
                    case RedisType.Unknown:
                    default:
                        throw new InvalidOperationException($"Type {keysPerType.Type} cannot be processed.");
                }
            }
            await backupSaver.SaveCommands(options.FileName, serializedCommads);
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
