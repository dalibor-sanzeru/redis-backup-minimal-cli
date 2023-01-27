using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RedisBackupMinimalCli
{
    public class BackupManager
    {
        private readonly Options options;

        public BackupManager(Options options)
        {
            this.options = options;
        }

        public Task Execute()
        {
            switch (options.Operation)
            {
                case OperationType.Backup:

                    break;
                case OperationType.Restore:

                    break;
                default:
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
