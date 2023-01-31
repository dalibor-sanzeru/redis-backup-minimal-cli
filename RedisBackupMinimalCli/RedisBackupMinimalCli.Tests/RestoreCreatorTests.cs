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
    }
}
