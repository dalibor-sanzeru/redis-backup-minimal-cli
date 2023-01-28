using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisBackupMinimalCli.Creators
{
    public abstract class CreatorBase
    {
        protected readonly IDatabase database;
        protected readonly IServer server;

        public CreatorBase(IServer server, IDatabase database)
        {
            this.server = server;
            this.database = database;
        }


        public abstract Task Execute(Options options);
    }
}
