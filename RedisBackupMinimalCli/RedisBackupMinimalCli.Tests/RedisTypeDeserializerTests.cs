﻿using FluentAssertions;
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
        [InlineData("LPUSH xyz", RedisType.List)]
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
    }
}
