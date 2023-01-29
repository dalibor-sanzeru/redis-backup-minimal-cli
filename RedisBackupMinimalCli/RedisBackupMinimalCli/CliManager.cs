﻿using RedisBackupMinimalCli.Creators;
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
            var (server, database) = ConnectToRedis(options.Redis);
            var creatorSaver = new FileBackupSaver();
            var redisTypeSerializer = new RedisTypeSerializer();

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

        private (IServer server, IDatabase database) ConnectToRedis(string redisConnection)
        {
            ConnectionMultiplexer client = ConnectionMultiplexer.Connect(redisConnection);
            var server = client.GetServer(redisConnection);
            var database = client.GetDatabase();

            return (server, database);
        }
    }
}
