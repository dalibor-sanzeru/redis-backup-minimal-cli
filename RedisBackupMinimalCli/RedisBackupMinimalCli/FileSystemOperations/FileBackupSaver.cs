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

        public Task Save(string directory, string fileName, List<string> results)
        {
            return File.WriteAllLinesAsync(Path.Combine(directory, $"{fileName}.{fileSuffix}"), results);
        }
    }
}
