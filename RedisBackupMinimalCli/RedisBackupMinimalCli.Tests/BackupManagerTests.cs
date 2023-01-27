using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace RedisBackupMinimalCli.Tests
{
    public class BackupManagerTests
    {
        [Fact]
        public void BackupManager_Ctor()
        {
            var bm = new BackupManager();
            bm.Should().NotBeNull();
        }

        [Fact]
        public async Task BackupManager_Operation_Backup_Stores_All_Data_Types()
        {
            var opt = new Options()
            {
                Directory = ".",
                Operation = OperationType.Restore,
                Redis = "localhost:6379",
                Keys = new List<string>
                {"TestDb:*"}
            };
            var bm = new BackupManager();
            await bm.Execute(opt);
        }
    }
}