using FluentAssertions;
using Moq;
using RedisBackupMinimalCli.Creators;
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
    public class RestoreCreatorTests
    {
        private static RestoreCreator CreateRestoreCreator(IServer server = null, IDatabase database = null, IRedisTypeDeserializer rts = null, ICommandPersistanceHandler bs = null)
        {
            var dbMock = new Mock<IDatabase>();
            var serverMock = new Mock<IServer>();
            var redisTypeSerializer = new Mock<IRedisTypeDeserializer>();
            var backupSaver = new Mock<ICommandPersistanceHandler>();

            var rc = new RestoreCreator(server ?? serverMock.Object, database ?? dbMock.Object, rts ?? redisTypeSerializer.Object, bs ?? backupSaver.Object);
            return rc;
        }


        [Fact]
        public void Ctor()
        {
            var rc = CreateRestoreCreator();
            rc.Should().NotBeNull();
        }

        [Fact]
        public async Task ReStore_All_Strings()
        {
            var sets = BackupCreatorTests.SourceTestData.Where(x => x.StartsWith(RedisTypeDeserializer.KeyTypeRedisCommand));

            var opt = new Options()
            {
                FileName = "./restore.redis",
                Operation = OperationType.Restore,
                Redis = "localhost:6379"
            };

            var server = new Mock<IServer>();
            var batch = new Mock<IBatch>();
            var db = new Mock<IDatabase>();
            db.Setup(x => x.CreateBatch(null)).Returns(batch.Object);

            var bs = new Mock<ICommandPersistanceHandler>();
            bs.Setup(x => x.LoadCommands(It.Is<string>(x => x == opt.FileName))).Returns(Task.FromResult(sets.ToList()));

            var rc = new RestoreCreator(server.Object, db.Object, new RedisTypeDeserializer(), bs.Object);
            await rc.Execute(opt);

            db.Verify(x => x.SetAddAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), CommandFlags.None), Times.Exactly(2));
            db.Verify(x => x.CreateBatch(null));
            batch.Verify(x => x.Execute(), Times.Once);
        }


        [Fact]
        public async Task ReStore_All_Hashes()
        {
            var sets = BackupCreatorTests.SourceTestData.Where(x => x.StartsWith(RedisTypeDeserializer.HashTypeRedisCommand));

            var opt = new Options()
            {
                FileName = "./restore.redis",
                Operation = OperationType.Restore,
                Redis = "localhost:6379"
            };

            var server = new Mock<IServer>();
            var batch = new Mock<IBatch>();
            var db = new Mock<IDatabase>();
            db.Setup(x => x.CreateBatch(null)).Returns(batch.Object);

            var bs = new Mock<ICommandPersistanceHandler>();
            bs.Setup(x => x.LoadCommands(It.Is<string>(x => x == opt.FileName))).Returns(Task.FromResult(sets.ToList()));

            var rc = new RestoreCreator(server.Object, db.Object, new RedisTypeDeserializer(), bs.Object);
            await rc.Execute(opt);

            db.Verify(x => x.HashSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<RedisValue>(), When.Always, CommandFlags.None), Times.Exactly(4));
            db.Verify(x => x.CreateBatch(null));
            batch.Verify(x => x.Execute(), Times.Once);
        }

        [Fact]
        public async Task ReStore_All_Lists()
        {
            var sets = BackupCreatorTests.SourceTestData.Where(x => x.StartsWith(RedisTypeDeserializer.ListTypeRedisCommand));

            var opt = new Options()
            {
                FileName = "./restore.redis",
                Operation = OperationType.Restore,
                Redis = "localhost:6379"
            };

            var server = new Mock<IServer>();
            var batch = new Mock<IBatch>();
            var db = new Mock<IDatabase>();
            db.Setup(x => x.CreateBatch(null)).Returns(batch.Object);

            var bs = new Mock<ICommandPersistanceHandler>();
            bs.Setup(x => x.LoadCommands(It.Is<string>(x => x == opt.FileName))).Returns(Task.FromResult(sets.ToList()));

            var rc = new RestoreCreator(server.Object, db.Object, new RedisTypeDeserializer(), bs.Object);
            await rc.Execute(opt);

            db.Verify(x => x.ListRightPushAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), When.Always, CommandFlags.None), Times.Exactly(4));
            db.Verify(x => x.CreateBatch(null));
            batch.Verify(x => x.Execute(), Times.Once);
        }

        [Fact]
        public async Task ReStore_All_Sets()
        {
            var sets = BackupCreatorTests.SourceTestData.Where(x => x.StartsWith(RedisTypeDeserializer.SetTypeRedisCommand));

            var opt = new Options()
            {
                FileName = "./restore.redis",
                Operation = OperationType.Restore,
                Redis = "localhost:6379"
            };

            var server = new Mock<IServer>();
            var batch = new Mock<IBatch>();
            var db = new Mock<IDatabase>();
            db.Setup(x => x.CreateBatch(null)).Returns(batch.Object);

            var bs = new Mock<ICommandPersistanceHandler>();
            bs.Setup(x => x.LoadCommands(It.Is<string>(x => x == opt.FileName))).Returns(Task.FromResult(sets.ToList()));

            var rc = new RestoreCreator(server.Object, db.Object, new RedisTypeDeserializer(), bs.Object);
            await rc.Execute(opt);

            db.Verify(x => x.SetAddAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), CommandFlags.None), Times.Exactly(4));
            db.Verify(x => x.CreateBatch(null));
            batch.Verify(x => x.Execute(), Times.Once);
        }

        [Fact]
        public async Task ReStore_All_SortedSets()
        {
            var sets = BackupCreatorTests.SourceTestData.Where(x => x.StartsWith(RedisTypeDeserializer.SortedSetRedisCommand));

            var opt = new Options()
            {
                FileName = "./restore.redis",
                Operation = OperationType.Restore,
                Redis = "localhost:6379"
            };

            var server = new Mock<IServer>();
            var batch = new Mock<IBatch>();
            var db = new Mock<IDatabase>();
            db.Setup(x => x.CreateBatch(null)).Returns(batch.Object);

            var bs = new Mock<ICommandPersistanceHandler>();
            bs.Setup(x => x.LoadCommands(It.Is<string>(x => x == opt.FileName))).Returns(Task.FromResult(sets.ToList()));

            var rc = new RestoreCreator(server.Object, db.Object, new RedisTypeDeserializer(), bs.Object);
            await rc.Execute(opt);

            db.Verify(x => x.SortedSetAddAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<double>(), SortedSetWhen.Always, CommandFlags.None), Times.Exactly(4));
            db.Verify(x => x.CreateBatch(null));
            batch.Verify(x => x.Execute(), Times.Once);
        }
    }
}
