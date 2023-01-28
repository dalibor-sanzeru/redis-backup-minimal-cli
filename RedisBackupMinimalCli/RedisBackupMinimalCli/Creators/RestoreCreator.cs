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
        public RestoreCreator(IServer server, IDatabase database) : base(server, database)
        {
        }

        public override Task Execute(Options options)
        {
            throw new NotImplementedException();
        }
    }
}
