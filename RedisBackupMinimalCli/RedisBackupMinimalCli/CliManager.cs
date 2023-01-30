using RedisBackupMinimalCli.Creators;
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

namespace RedisBackupMinimalCli
{
    public class CliManager
    {
        public async Task Execute(Options options)
        {
            var (server, database) = CreateDatabaseConnection(options.Redis);
            var creatorSaver = this.CreateBackupSaver();
            var redisTypeSerializer = CreateRedisTypeSerializer();

            switch (options.Operation)
            {
                case OperationType.Backup:
                    await new BackupCreator(server, database, redisTypeSerializer, creatorSaver).Execute(options);
                    break;
                case OperationType.Restore:
                    await new RestoreCreator(server, database).Execute(options);
                    break;
                default:
                    throw new InvalidOperationException($"Operation {options.Operation} not yet supported.");
            }
        }

        protected virtual (IServer server, IDatabase database) CreateDatabaseConnection(string redisConnection)
        {
            ConnectionMultiplexer client = ConnectionMultiplexer.Connect(redisConnection);
            var server = client.GetServer(redisConnection);
            var database = client.GetDatabase();

            return (server, database);
        }

        protected virtual IBackupSaver CreateBackupSaver()
        {
            return new FileBackupSaver();
        }

        protected virtual IRedisTypeSerializer CreateRedisTypeSerializer()
        {
            return new RedisTypeSerializer();
        }
    }
}
