using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Moq;
using RedisBackupMinimalCli.Creators;
using RedisBackupMinimalCli.FileSystemOperations;
using RedisBackupMinimalCli.Serialization;
using StackExchange.Redis;

namespace RedisBackupMinimalCli.Tests
{
    public class BackupCreatorTests
    {
        private BackupCreator CreateBackupCreator(IServer server = null, IDatabase database = null, IRedisTypeSerializer rts = null, IBackupSaver bs = null)
        {
            var dbMock = new Mock<IDatabase>();
            var serverMock = new Mock<IServer>();
            var redisTypeSerializer = new Mock<IRedisTypeSerializer>();
            var backupSaver = new Mock<IBackupSaver>();

            var bm = new BackupCreator(server ?? serverMock.Object, database ?? dbMock.Object, rts ?? redisTypeSerializer.Object, bs ?? backupSaver.Object);
            return bm;
        }

        [Fact]
        public void Ctor()
        {
            var bc = this.CreateBackupCreator();
            bc.Should().NotBeNull();
        }

        [Fact]
        public async Task Stores_All_Supported_Types()
        {
            var opt = new Options()
            {
                Directory = ".",
                Operation = OperationType.Backup,
                Redis = "localhost:6379",
                Keys = new List<string>
                {"TestDb:*"}
            };

            var redis = ConnectionMultiplexer.Connect(opt.Redis);
            var bs = new Mock<IBackupSaver>();

            var bm = this.CreateBackupCreator(redis.GetServer(opt.Redis), redis.GetDatabase(), new RedisTypeSerializer(), bs.Object);
            await bm.Execute(opt);
            bs.Verify(x => x.Save(It.Is<string>(x => x == opt.Directory), It.Is<List<string>>(x => x.Count >= 2)), Times.Exactly(5));
        }

        [Fact]
        public async Task Stores_All_Hashes()
        {
            var opt = new Options()
            {
                Directory = ".",
                Operation = OperationType.Backup,
                Redis = "localhost:6379",
                Keys = new List<string>
                {"TestDb:Hashes:*"}
            };

            var redis = ConnectionMultiplexer.Connect(opt.Redis);
            var bs = new Mock<IBackupSaver>();

            var bm = this.CreateBackupCreator(redis.GetServer(opt.Redis), redis.GetDatabase(), new RedisTypeSerializer(), bs.Object);
            await bm.Execute(opt);

            bs.Verify(x => x.Save(It.Is<string>(x => x == opt.Directory), It.Is<List<string>>(x => x.Count == 4)), Times.Once());
        }

        [Fact]
        public async Task Store_All_Strings()
        {
            var opt = new Options()
            {
                Directory = ".",
                Operation = OperationType.Backup,
                Redis = "localhost:6379",
                Keys = new List<string>
                {"TestDb:Keys:*"}
            };

            var redis = ConnectionMultiplexer.Connect(opt.Redis);
            var bs = new Mock<IBackupSaver>();

            var bm = this.CreateBackupCreator(redis.GetServer(opt.Redis), redis.GetDatabase(), new RedisTypeSerializer(), bs.Object);
            await bm.Execute(opt);

            bs.Verify(x => x.Save(It.Is<string>(x => x == opt.Directory), It.Is<List<string>>(x => x.Count == 2)), Times.Once());
        }

        [Fact]
        public async Task Store_All_Sets()
        {
            var opt = new Options()
            {
                Directory = ".",
                Operation = OperationType.Backup,
                Redis = "localhost:6379",
                Keys = new List<string>
                {"TestDb:Sets:*"}
            };

            var redis = ConnectionMultiplexer.Connect(opt.Redis);
            var bs = new Mock<IBackupSaver>();

            var bm = this.CreateBackupCreator(redis.GetServer(opt.Redis), redis.GetDatabase(), new RedisTypeSerializer(), bs.Object);
            await bm.Execute(opt);

            bs.Verify(x => x.Save(It.Is<string>(x => x == opt.Directory), It.Is<List<string>>(x => x.Count == 4)), Times.Once());
        }

        [Fact]
        public async Task Store_All_SortedSets()
        {
            var opt = new Options()
            {
                Directory = ".",
                Operation = OperationType.Backup,
                Redis = "localhost:6379",
                Keys = new List<string>
                {"TestDb:SortedSets:*"}
            };

            var redis = ConnectionMultiplexer.Connect(opt.Redis);
            var bs = new Mock<IBackupSaver>();

            var bm = this.CreateBackupCreator(redis.GetServer(opt.Redis), redis.GetDatabase(), new RedisTypeSerializer(), bs.Object);
            await bm.Execute(opt);

            bs.Verify(x => x.Save(It.Is<string>(x => x == opt.Directory), It.Is<List<string>>(x => x.Count == 4)), Times.Once());
        }

        [Fact(Skip = "Streams are not supported yet. Problem with local running stream commands on redis.")]

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

            var redis = ConnectionMultiplexer.Connect(opt.Redis);
            var bs = new Mock<IBackupSaver>();

            var bm = this.CreateBackupCreator(redis.GetServer(opt.Redis), redis.GetDatabase(), new RedisTypeSerializer(), bs.Object);
            await bm.Execute(opt);

            bs.Verify(x => x.Save(It.Is<string>(x => x == opt.Directory), It.Is<List<string>>(x => x.Count == 2)), Times.Once());
        }
    }
}