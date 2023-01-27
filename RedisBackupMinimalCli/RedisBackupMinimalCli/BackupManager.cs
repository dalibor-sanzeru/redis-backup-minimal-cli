using StackExchange.Redis;
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
        }
    }
}
