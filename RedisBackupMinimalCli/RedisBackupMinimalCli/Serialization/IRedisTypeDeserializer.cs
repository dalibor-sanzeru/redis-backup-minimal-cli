using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisBackupMinimalCli.Serialization
{
    public interface IRedisTypeDeserializer
    {
        RedisType GetRedisType(string command);

        List<KeyValuePair<string, RedisValue>> DeSerializeStrings(List<string> commands);
    }
}
