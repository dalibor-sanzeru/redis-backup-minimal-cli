using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisBackupMinimalCli.Serialization
{
    public class ListsSerializer : ItemsSerializerBase<RedisValue[]>
    {
        protected override List<string> SerializeItem(KeyValuePair<string, RedisValue[]> item)
        {
            throw new NotImplementedException();
        }
    }
}
