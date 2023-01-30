using Moq;
using RedisBackupMinimalCli.FileSystemOperations;
using RedisBackupMinimalCli.Serialization;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisBackupMinimalCli.Tests
{
    public class CliManagerTests
    {
        [Fact]
        public async Task Stores_All_Supported_Types()
        {
            var opt = new Options()
            {
                Directory = ".",
                Operation = OperationType.Backup,
                Redis = "localhost:6379",
                Keys = new List<string>
                {"TestDb:*"}
            };

            var cli = new CliManager();
            await cli.Execute(opt);
        }
    }
}
