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
                Directory = ".",
                FileName = "backup.redis",
                Operation = OperationType.Backup,
                Redis = "localhost:6379",
                Keys = new List<string>
                {"TestDb:*"}
            };

            var cli = new CliManager();
            await cli.Execute(opt);

            string p = Path.Combine(opt.Directory, opt.FileName);
            var savedResults = File.ReadAllLines(p);
            savedResults.Count().Should().Be(BackupCreatorTests.SourceTestData.Count(x => !x.StartsWith("XADD")));
        }

        [Fact]
        public async Task Stores_All_Supported_Types_WithGenerated_FileName()
        {
            var opt = new Options()
            {
                Directory = ".",
                Operation = OperationType.Backup,
                Redis = "localhost:6379",
                Keys = new List<string>
                {"TestDb:*"}
            };

            var cli = new CliManager();
            await cli.Execute(opt);

            var lastFileCreated = Directory.EnumerateFiles(opt.Directory, "*.redis")
                .Select(f => new { Filename = f, CreatedOn = File.GetCreationTimeUtc(f) }).OrderByDescending(x=>x.CreatedOn)
                .First()
                .Filename;

            var savedResults = File.ReadAllLines(lastFileCreated);
            savedResults.Count().Should().Be(BackupCreatorTests.SourceTestData.Count(x => !x.StartsWith("XADD")));
        }
    }
}
