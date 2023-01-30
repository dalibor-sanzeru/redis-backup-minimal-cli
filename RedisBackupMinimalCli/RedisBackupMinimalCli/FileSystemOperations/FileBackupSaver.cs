using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisBackupMinimalCli.FileSystemOperations
{
    public class FileBackupSaver : IBackupSaver
    {
        private readonly string fileSuffix;

        public FileBackupSaver(string fileSuffix = "redis")
        {
            this.fileSuffix = fileSuffix;
        }

        public Task Save(string directory, string fileName, List<string> serializedCommads)
        {
            var defaultName = $"{DateTime.UtcNow.ToString("MMddyy_HHmmss")}.redis" ;
            var path = Path.Combine(directory, $"{fileName ?? defaultName}");

            return File.WriteAllLinesAsync(path, serializedCommads);
        }
    }
}
