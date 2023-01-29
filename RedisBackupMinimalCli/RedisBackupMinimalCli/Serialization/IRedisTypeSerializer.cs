using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisBackupMinimalCli.Serialization
{
    public interface IRedisTypeSerializer
    {
        List<string> SerializeHashSets(List<KeyValuePair<string, HashEntry[]>> items);
        
        List<string> SerializeStrings(List<KeyValuePair<string, RedisValue>> items);
        
        List<string> SerializeLists(List<KeyValuePair<string, RedisValue[]>> items);
        
        List<string> SerializeSets(List<KeyValuePair<string, RedisValue[]>> items);

        List<string> SerializeSortedSets(List<KeyValuePair<string, SortedSetEntry[]>> items);
        
        List<string> SerializeStreams(List<KeyValuePair<string, StreamEntry[]>> items);
    }
}
