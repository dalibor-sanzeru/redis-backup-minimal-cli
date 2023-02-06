using RedisBackupMinimalCli.PersistanceOperations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisBackupMinimalCli.Tests
{
    public class CommandFileHandlerTests
    {
        [Fact]
        public async Task SaveCommands_FileName_Provided()
        {
            var commandFileHandler = new CommandFileHandler();
            await commandFileHandler.SaveCommands("./result.txt", new List<string>());
        }
    }
}
