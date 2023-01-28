using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using StackExchange.Redis;

namespace RedisBackupMinimalCli.Tests
{
    public class BackupManagerTests
    {
        [Fact]
        public void BackupManager_Ctor()
        {
            var bm = new CliManager();
            bm.Should().NotBeNull();
        }

        [Fact]
        public async Task BackupManager_Operation_Backup_Stores_All_Data_Types()
        {
            var opt = new Options()
            {
                Directory = ".",
                Operation = OperationType.Backup,
                Redis = "localhost:6379",
                Keys = new List<string>
                {"TestDb:*"}
            };
            var bm = new CliManager();
            await bm.Execute(opt);
        }

        [Fact]
        public async Task BackupManager_Operation_Backup_Stores_All_Hashes()
        {
            var opt = new Options()
            {
                Directory = ".",
                Operation = OperationType.Backup,
                Redis = "localhost:6379",
                Keys = new List<string>
                {"TestDb:Hashes:*"}
            };
            var bm = new CliManager();
            await bm.Execute(opt);
        }

        [Fact]
        public async Task BackupManager_Operation_Backup_Stores_All_Sets()
        {
            var opt = new Options()
            {
                Directory = ".",
                Operation = OperationType.Backup,
                Redis = "localhost:6379",
                Keys = new List<string>
                {"TestDb:Sets:*"}
            };
            var bm = new CliManager();
            await bm.Execute(opt);
        }

        [Fact]
        public async Task BackupManager_Operation_Backup_Stores_All_SortedSets()
        {
            var opt = new Options()
            {
                Directory = ".",
                Operation = OperationType.Backup,
                Redis = "localhost:6379",
                Keys = new List<string>
                {"TestDb:SortedSets:*"}
            };
            var bm = new CliManager();
            await bm.Execute(opt);
        }

        [Fact]
        public async Task BackupManager_Operation_Backup_Stores_All_Streams()
        {
            var opt = new Options()
            {
                Directory = ".",
                Operation = OperationType.Backup,
                Redis = "localhost:6379",
                Keys = new List<string>
                {"TestDb:Streams:*"}
            };
            var bm = new CliManager();
            await bm.Execute(opt);
        }
    }
}