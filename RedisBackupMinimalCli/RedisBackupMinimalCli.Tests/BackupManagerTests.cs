using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace RedisBackupMinimalCli.Tests
{
    public class BackupManagerTests
    {
        [Fact]
        public void BackupManager_Ctor()
        {
            var opt = new Options();
            var bm = new BackupManager(opt);

            bm.Should().NotBeNull();
        }

        [Fact]
        public async Task BackupManager_Operation_Backup_Stores_All_Data_Types()
        {
            var opt = new Options();
            var bm = new BackupManager(opt);
            await bm.Execute();
        }

    }
}