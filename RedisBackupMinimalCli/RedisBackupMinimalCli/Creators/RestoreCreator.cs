using RedisBackupMinimalCli.PersistanceOperations;
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

        public RestoreCreator(IServer server, IDatabase database, ICommandPersistanceHandler commandPersistanceHandler) : base(server, database)
        {
            this.commandPersistanceHandler = commandPersistanceHandler;
        }

        public override Task Execute(Options options)
        {
            var commands = this.commandPersistanceHandler.LoadCommands(options.FileName);

            throw new NotImplementedException();
        }
    }
}
