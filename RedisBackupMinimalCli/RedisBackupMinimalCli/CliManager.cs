using RedisBackupMinimalCli.Creators;
using RedisBackupMinimalCli.PersistanceOperations;
using RedisBackupMinimalCli.Serialization;
using StackExchange.Redis;

namespace RedisBackupMinimalCli
{
    public class CliManager
    {
        public async Task Execute(Options options)
        {
            var (server, database) = CreateDatabaseConnection(options.Redis);
            var persistanceHandler = this.CreateCommandPersistanceHandler();
            var redisTypeSerializer = CreateRedisTypeSerializer();
            var redisTypeDeserializer = CreateRedisTypeDeSerializer();

            switch (options.Operation)
            {
                case OperationType.Backup:
                    await new BackupCreator(server, database, redisTypeSerializer, persistanceHandler).Execute(options);
                    break;
                case OperationType.Restore:
                    await new RestoreCreator(server, database, redisTypeDeserializer, persistanceHandler).Execute(options);
                    break;
                default:
                    throw new InvalidOperationException($"Operation {options.Operation} not yet supported.");
            }
        }

        protected virtual (IServer server, IDatabase database) CreateDatabaseConnection(string redisConnection)
        {
            ConnectionMultiplexer client = ConnectionMultiplexer.Connect(redisConnection);
            string hostAndPort = redisConnection.Split(',')[0];
            var server = client.GetServer(hostAndPort);
            var database = client.GetDatabase();

            return (server, database);
        }

        protected virtual ICommandPersistanceHandler CreateCommandPersistanceHandler()
        {
            return new CommandFileHandler();
        }

        protected virtual IRedisTypeSerializer CreateRedisTypeSerializer()
        {
            return new RedisTypeSerializer();
        }

        protected virtual IRedisTypeDeserializer CreateRedisTypeDeSerializer()
        {
            return new RedisTypeDeserializer();
        }
    }
}
