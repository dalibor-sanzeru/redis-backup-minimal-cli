using FluentAssertions;
using Moq;
using RedisBackupMinimalCli.PersistanceOperations;
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
        public async Task Stores_All_Supported_Types_WithUser_FileName()
        {
            var opt = new Options()
            {
                FileName = "./backup.redis",
                Operation = OperationType.Backup,
                Redis = "localhost:6379",
                Keys = new List<string>
                {"TestDb:*"}
            };

            var cli = new CliManager();
            await cli.Execute(opt);

            var savedResults = File.ReadAllLines(opt.FileName);
            savedResults.Count().Should().Be(BackupCreatorTests.SourceTestData.Count(x => !x.StartsWith("XADD")));
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Stores_All_Supported_Types_WithGenerated_FileName(string emptyFilename)
        {
            var opt = new Options()
            {
                FileName = emptyFilename,
                Operation = OperationType.Backup,
                Redis = "localhost:6379",
                Keys = new List<string>
                {"TestDb:*"}
            };

            var cli = new CliManager();
            await cli.Execute(opt);

            var lastFileCreated = Directory.EnumerateFiles(".", "*.redis")
                .Select(f => new { Filename = f, CreatedOn = File.GetCreationTimeUtc(f) }).OrderByDescending(x => x.CreatedOn)
                .First()
                .Filename;

            var savedResults = File.ReadAllLines(lastFileCreated);
            savedResults.Count().Should().Be(BackupCreatorTests.SourceTestData.Count(x => !x.StartsWith("XADD")));
        }

        [Fact]
        public async Task Restore_All_Supported_Types_WithUser_FileName()
        {
            var opt = new Options()
            {
                FileName = "./redis-test-data.txt",
                Operation = OperationType.Restore,
                Redis = "localhost:6379"
            };

            var cli = new CliManager();
            await cli.Execute(opt);

            //var savedResults = File.ReadAllLines(opt.FileName);
            //savedResults.Count().Should().Be(BackupCreatorTests.SourceTestData.Count(x => !x.StartsWith("XADD")));
        }
    }
}
