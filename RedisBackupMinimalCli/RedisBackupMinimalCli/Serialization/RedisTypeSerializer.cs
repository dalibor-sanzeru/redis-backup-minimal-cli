using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisBackupMinimalCli.Serialization
{
    public class RedisTypeSerializer : IRedisTypeSerializer
    {
        private List<string> Serialize<ItemsType>(List<KeyValuePair<string, ItemsType>> items, Func<KeyValuePair<string, ItemsType>, List<string>> converter)
        {
            List<string> result = new();
            for (int i = 0; i < items.Count; i++)
            {
                result.AddRange(converter(items[i]));
            }

            return result;
        }

        public List<string> SerializeHashSets(List<KeyValuePair<string, HashEntry[]>> items)
        {
            return this.Serialize(items, (item) =>
            {
                return item.Value.Select(x => @$"HSET ""{item.Key}"" ""{x.Name}"" ""{x.Value}""").ToList();
            });
        }

        public List<string> SerializeStrings(List<KeyValuePair<string, RedisValue>> items)
        {
            return this.Serialize(items, (item) =>
            {
                return new List<string>() { @$"SET ""{item.Key}"" ""{item.Value}""" };
            });
        }

        public List<string> SerializeLists(List<KeyValuePair<string, RedisValue[]>> items)
        {
            return this.Serialize(items, (item) =>
            {
                return item.Value.Select(x => @$"LPUSH ""{item.Key}"" ""{x}""").ToList();
            });
        }

        public List<string> SerializeSets(List<KeyValuePair<string, RedisValue[]>> items)
        {
            return this.Serialize(items, (item) =>
            {
                return item.Value.Select(x => @$"SADD ""{item.Key}"" ""{x}""").ToList();
            });
        }

        public List<string> SerializeSortedSets(List<KeyValuePair<string, SortedSetEntry[]>> items)
        {
            return this.Serialize(items, (item) =>
            {
                return item.Value.Select(x => @$"ZADD ""{item.Key}"" {x.Score} ""{x.Element}""").ToList();
            });
        }

        public List<string> SerializeStreams(List<KeyValuePair<string, StreamEntry[]>> items)
        {
            throw new NotImplementedException();
        }
    }
}
