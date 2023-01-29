using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisBackupMinimalCli.FileSystemOperations
{
    public class FileBackupSaver : IBackupSaver
    {
        public FileBackupSaver() { }

        public Task Save(string path, List<string> results)
        {
            return File.WriteAllLinesAsync(path, results);
        }
    }
}
