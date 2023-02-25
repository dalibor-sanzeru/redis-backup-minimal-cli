using FluentAssertions;
using RedisBackupMinimalCli.Serialization;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisBackupMinimalCli.Tests
{
    public class RedisTypeDeserializerTests
    {
        [Fact]
        public void Ctor()
        {
            var redisTypeSerializer = new RedisTypeDeserializer();
            redisTypeSerializer.Should().NotBeNull();
        }

        [Theory]
        [InlineData("set smallcaps", RedisType.String)]
        [InlineData("SET xyz", RedisType.String)]
        [InlineData("HSET xyz", RedisType.Hash)]
        [InlineData("RPUSH xyz", RedisType.List)]
        [InlineData("SADD xyz", RedisType.Set)]
        [InlineData("ZADD xyz", RedisType.SortedSet)]
        [InlineData("XADD xyz", RedisType.Stream)]
        [InlineData("XADD_xyz", RedisType.Unknown)]
        [InlineData("", RedisType.Unknown)]
        [InlineData(" Invalid", RedisType.Unknown)]
        public void RedisGetType_ParsedCorrectly(string command, RedisType expectedRedisType)
        {
            var redisTypeSerializer = new RedisTypeDeserializer();
            var res = redisTypeSerializer.GetRedisType(command);

            res.Should().Be(expectedRedisType);
        }

        [Theory]
        [InlineData("  SET    \"key\"     \"value\"     ")]
        [InlineData("SET \"key\" \"value\"")]
        public void RedisGetType_DeSerializeString(string command)
        {
            var redisTypeSerializer = new RedisTypeDeserializer();
            var res = redisTypeSerializer.DeSerializeString(command);

            res.Key.Should().Be("key");
            res.Value.Should().Be("value");
        }

        [Theory]
        [InlineData("  HSET    \"key\"    \"subkey\"     \"value\"     ")]
        [InlineData("HSET \"key\" \"subkey\" \"value\"")]
        public void RedisGetType_DeSerializeHash(string command)
        {
            var redisTypeSerializer = new RedisTypeDeserializer();
            var res = redisTypeSerializer.DeSerializeHash(command);

            res.Key.Should().Be("key");
            res.Value.Name.Should().Be("subkey");
            res.Value.Value.Should().Be("value");
        }

        [Theory]
        [InlineData("  SADD    \"key\"    \"value\" ")]
        [InlineData("SADD \"key\" \"value\"")]
        public void RedisGetType_DeSerializeSet(string command)
        {
            var redisTypeSerializer = new RedisTypeDeserializer();
            var res = redisTypeSerializer.DeSerializeSet(command);

            res.Key.Should().Be("key");
            res.Value.Should().Be("value");
        }

        [Theory]
        [InlineData("  RPUSH    \"key\"    \"value1\"")]
        [InlineData("RPUSH \"key\" \"value1\" ")]
        public void RedisGetType_DeSerializeList_OneItem(string command)
        {
            var redisTypeSerializer = new RedisTypeDeserializer();
            var res = redisTypeSerializer.DeSerializeList(command);

            res.Key.Should().Be("key");
            res.Value[0].Should().Be("value1");
        }

        [Theory]
        [InlineData("  RPUSH    \"key\"    \"value1\"     \"value2\"")]
        [InlineData("RPUSH \"key\" \"value1\" \"value2\"")]
        public void RedisGetType_DeSerializeList_MultipleItems(string command)
        {
            var redisTypeSerializer = new RedisTypeDeserializer();
            var res = redisTypeSerializer.DeSerializeList(command);

            res.Key.Should().Be("key");
            res.Value[0].Should().Be("value1");
            res.Value[1].Should().Be("value2");
        }

        [Theory]
        [InlineData("  RPUSH \"key\"    \"{\"Name\":\"Alfa\"}\"     \"{\"Name\":\"Beta\"}\"     \"{\"Name\":\"Gama\"}\"")]
        [InlineData("RPUSH \"key\" \"{\"Name\":\"Alfa\"}\" \"{\"Name\":\"Beta\"}\" \"{\"Name\":\"Gama\"}\"")]
        public void RedisGetType_DeSerializeList_MUltipleJsons(string command)
        {
            var redisTypeSerializer = new RedisTypeDeserializer();
            var res = redisTypeSerializer.DeSerializeList(command);

            res.Key.Should().Be("key");
            res.Value[0].Should().Be(@"{""Name"":""Alfa""}");
            res.Value[1].Should().Be(@"{""Name"":""Beta""}");
            res.Value[2].Should().Be(@"{""Name"":""Gama""}");
        }

        [Theory]
        [InlineData("  ZADD    \"key\"    10     \"value\"     ")]
        [InlineData("ZADD \"key\" 10 \"value\"")]
        public void RedisGetType_DeSerializeSortedSet(string command)
        {
            var redisTypeSerializer = new RedisTypeDeserializer();
            var res = redisTypeSerializer.DeSerializeSortedSet(command);

            res.Key.Should().Be("key");
            res.Value.Score.Should().Be(10);
            res.Value.Element.Should().Be("value");
        }

        [Theory]
        [InlineData("  XADD    \"key\"    \"Speed\"     \"10\"     ")]
        [InlineData("XADD \"key\" \"Speed\" \"10\"")]
        public void RedisGetType_DeSerializeStream(string command)
        {
            var redisTypeSerializer = new RedisTypeDeserializer();
            Assert.Throws<NotImplementedException>(() => redisTypeSerializer.DeSerializeStream(command));
        }
    }
}
