using FluentAssertions;
using RedisBackupMinimalCli.FileSystemOperations;
using RedisBackupMinimalCli.Serialization;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisBackupMinimalCli.Tests
{
    public class RedisTypeSerializerTests
    {
        [Fact]
        public void Ctor()
        {
            var redisTypeSerializer = new RedisTypeSerializer();
            redisTypeSerializer.Should().NotBeNull();
        }

        [Fact]
        public void Serialize_Empty_Keys()
        {
            var redisTypeSerializer = new RedisTypeSerializer();
            var res = redisTypeSerializer.SerializeStrings(new List<KeyValuePair<string, RedisValue>>());
            res.Should().BeEmpty();
        }

        [Fact]
        public void Serialize_Multiple_Keys()
        {
            var redisTypeSerializer = new RedisTypeSerializer();

            var val = new List<KeyValuePair<string, RedisValue>>()
            {
                new KeyValuePair<string, RedisValue>("key1", "val1"),
                new KeyValuePair<string, RedisValue>("key2", "val2")
            };
            var res = redisTypeSerializer.SerializeStrings(val);
            res[0].Should().Be(@"SET ""key1"" ""val1""");
            res[1].Should().Be(@"SET ""key2"" ""val2""");
            res.Count.Should().Be(val.Count);
        }

        [Fact]
        public void Serialize_Empty_HashSets()
        {
            var redisTypeSerializer = new RedisTypeSerializer();
            var res = redisTypeSerializer.SerializeHashSets(new List<KeyValuePair<string, HashEntry[]>>());
            res.Should().BeEmpty();
        }

        [Fact]
        public void Serialize_Multiple_Hashsets()
        {
            var redisTypeSerializer = new RedisTypeSerializer();

            var val = new List<KeyValuePair<string, HashEntry[]>>()
            {
                new KeyValuePair<string, HashEntry[]>("key1", new HashEntry[]
                {
                    new HashEntry("1", "10"),
                    new HashEntry("2", "20")
                })
            };

            var res = redisTypeSerializer.SerializeHashSets(val);
            res[0].Should().Be(@"HSET ""key1"" ""1"" ""10""");
            res[1].Should().Be(@"HSET ""key1"" ""2"" ""20""");
            res.Count.Should().Be(2);
        }

        [Fact]
        public void Serialize_Empty_Lists()
        {
            var redisTypeSerializer = new RedisTypeSerializer();
            var res = redisTypeSerializer.SerializeLists(new List<KeyValuePair<string, RedisValue[]>>());
            res.Should().BeEmpty();
        }

        [Fact]
        public void Serialize_Multiple_Lists()
        {
            var redisTypeSerializer = new RedisTypeSerializer();

            var val = new List<KeyValuePair<string, RedisValue[]>>()
            {
                new KeyValuePair<string, RedisValue[]>("key1", new RedisValue[]
                {
                    "1",
                    "2"
                })
            };

            var res = redisTypeSerializer.SerializeLists(val);
            res[0].Should().Be(@"LPUSH ""key1"" ""1""");
            res[1].Should().Be(@"LPUSH ""key1"" ""2""");
            res.Count.Should().Be(2);
        }

        [Fact]
        public void Serialize_Empty_Sets()
        {
            var redisTypeSerializer = new RedisTypeSerializer();
            var res = redisTypeSerializer.SerializeSets(new List<KeyValuePair<string, RedisValue[]>>());
            res.Should().BeEmpty();
        }

        [Fact]
        public void Serialize_Multiple_Sets()
        {
            var redisTypeSerializer = new RedisTypeSerializer();

            var val = new List<KeyValuePair<string, RedisValue[]>>()
            {
                new KeyValuePair<string, RedisValue[]>("key1", new RedisValue[]
                {
                    "1",
                    "2"
                })
            };

            var res = redisTypeSerializer.SerializeSets(val);
            res[0].Should().Be(@"SADD ""key1"" ""1""");
            res[1].Should().Be(@"SADD ""key1"" ""2""");
            res.Count.Should().Be(2);
        }

        [Fact]
        public void Serialize_Empty_SortedSets()
        {
            var redisTypeSerializer = new RedisTypeSerializer();
            var res = redisTypeSerializer.SerializeSortedSets(new List<KeyValuePair<string, SortedSetEntry[]>>());
            res.Should().BeEmpty();
        }

        [Fact]
        public void Serialize_Multiple_SortedSets()
        {
            var redisTypeSerializer = new RedisTypeSerializer();

            var val = new List<KeyValuePair<string, SortedSetEntry[]>>
            {
                new KeyValuePair<string, SortedSetEntry[]>("key1", new SortedSetEntry[]
                {
                    new SortedSetEntry("1", 10),
                    new SortedSetEntry("2", 20)
                })
            };

            var res = redisTypeSerializer.SerializeSortedSets(val);
            res[0].Should().Be(@"ZADD ""key1"" 10 ""1""");
            res[1].Should().Be(@"ZADD ""key1"" 20 ""2""");
            res.Count.Should().Be(2);
        }

        [Fact]
        public void Serialize_Empty_Streams_Are_Not_Implemented_Yet()
        {
            var redisTypeSerializer = new RedisTypeSerializer();
            Assert.Throws<NotImplementedException>(() => redisTypeSerializer.SerializeStreams(new List<KeyValuePair<string, StreamEntry[]>>()));
        }
    }
}
