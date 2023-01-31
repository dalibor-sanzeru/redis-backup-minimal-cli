using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisBackupMinimalCli.PersistanceOperations
{
    public class CommandFileHandler : ICommandPersistanceHandler
    {
        private readonly string fileSuffix;

        public CommandFileHandler(string fileSuffix = "redis")
        {
            this.fileSuffix = fileSuffix;
        }

        public async Task<List<string>> LoadCommands(string fileName)
        {
            return (await File.ReadAllLinesAsync(fileName)).ToList();
        }

        public Task SaveCommands(string directory, string fileName, List<string> serializedCommads)
        {
            var defaultName = $"{DateTime.UtcNow.ToString("MMddyy_HHmmss")}.redis" ;
            var path = Path.Combine(directory, $"{fileName ?? defaultName}");

            return File.WriteAllLinesAsync(path, serializedCommads);
        }
    }
}
