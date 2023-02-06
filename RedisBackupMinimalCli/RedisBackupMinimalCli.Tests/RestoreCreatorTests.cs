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

        //[Fact]
        //public async Task Restores_All_Hashes()
        //{
        //    var opt = new Options()
        //    {
        //        Directory = ".",
        //        Operation = OperationType.Backup,
        //        Redis = "localhost:6379"
        //    };

        //    var redis = ConnectionMultiplexer.Connect(opt.Redis);
        //    var bs = new Mock<ICommandPersistanceHandler>();

        //    var bm = CreateRestoreCreator(redis.GetServer(opt.Redis), redis.GetDatabase(), new RedisTypeDeserializer(), bs.Object);
        //    await bm.Execute(opt);

        //    throw new Exception();

        //    bs.Verify(x => x.SaveCommands(It.Is<string>(x => x == opt.Directory), It.IsAny<string>(), It.Is<List<string>>(x => x.Count == 4)), Times.Once());
        //}
    }
}
