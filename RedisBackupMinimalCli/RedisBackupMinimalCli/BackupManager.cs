﻿using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RedisBackupMinimalCli
{
    public class BackupManager
    {
        public BackupManager()
        {
        }

        public async Task Execute(Options options)
        {
            switch (options.Operation)
            {
                case OperationType.Backup:
                    await this.ExecuteBackup(options);
                    break;
                case OperationType.Restore:
                    await this.ExecuteRestore(options);
                    break;
                default:
                    throw new InvalidOperationException($"Operation {options.Operation} not yet supported.");
            }
        }

        private (IServer server, IDatabase database) ConnectToRedis(string redisConnection)
        {
            ConnectionMultiplexer client = ConnectionMultiplexer.Connect(redisConnection);
            var server = client.GetServer(redisConnection);
            var database = client.GetDatabase();

            return (server, database);
        }

        private async Task ExecuteBackup(Options options)
        {
        }

        private async Task ExecuteRestore(Options options)
        {
            (IServer server, IDatabase database) = this.ConnectToRedis(options.Redis);

            //get keys with datatypes
            var batch = database.CreateBatch();
            var allKeys = options.Keys.SelectMany(k => server.Keys(pattern: k));
            var keysWithTypes = allKeys.ToDictionary(k => k.ToString(), k => batch.KeyTypeAsync(k));
            batch.Execute();
            await Task.WhenAll(keysWithTypes.Values);
            var redisTypeKeys = keysWithTypes
                                 .GroupBy(x => x.Value.Result)
                                 .Select(x => new { RedisType = x.Key, Keys = x.Select(x => x.Key).ToList() })
                                 .ToList();

            List<Task<RedisValue>> keyResults = new List<Task<RedisValue>>();
            List<Task<HashEntry[]>> hashResults = new List<Task<HashEntry[]>>();
            List<Task<RedisValue[]>> setResults = new List<Task<RedisValue[]>>();
            List<Task<SortedSetEntry[]>> sortedSetResults = new List<Task<SortedSetEntry[]>>();
            List<Task<RedisValue[]>> listResults = new List<Task<RedisValue[]>>();
            List<Task<StreamEntry[]>> streamResults = new List<Task<StreamEntry[]>>();

            batch = database.CreateBatch();
            redisTypeKeys.ForEach(keysPerType =>
            {
                switch (keysPerType.RedisType)
                {
                    case RedisType.String:
                        keyResults = keysPerType.Keys.Select(individualKey => batch.StringGetAsync(individualKey)).ToList();
                        break;
                    case RedisType.List:
                        listResults = keysPerType.Keys.Select(individualKey => batch.ListRangeAsync(individualKey)).ToList();
                        break;
                    case RedisType.Set:
                        setResults = keysPerType.Keys.Select(individualKey => batch.SetMembersAsync(individualKey)).ToList();
                        break;
                    case RedisType.SortedSet:
                        sortedSetResults = keysPerType.Keys.Select(individualKey => batch.SortedSetRangeByRankWithScoresAsync(individualKey)).ToList();
                        break;
                    case RedisType.Hash:
                        hashResults = keysPerType.Keys.Select(individualKey => batch.HashGetAllAsync(individualKey)).ToList();
                        break;
                    case RedisType.Stream:
                        streamResults = keysPerType.Keys.Select(individualKey => batch.StreamRangeAsync(individualKey)).ToList();
                        break;
                    case RedisType.None:
                    case RedisType.Unknown:
                    default:
                        throw new InvalidOperationException($"Type {keysPerType.RedisType} cannot be processed.");
                }
            });

            batch.Execute();
            await Task.WhenAll(keyResults);
            await Task.WhenAll(hashResults);
            await Task.WhenAll(setResults);
            await Task.WhenAll(sortedSetResults);
            await Task.WhenAll(listResults);
            await Task.WhenAll(streamResults);
        }
    }
}
