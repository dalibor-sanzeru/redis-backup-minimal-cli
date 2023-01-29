using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisBackupMinimalCli.Serialization
{
    public abstract class ItemsSerializerBase<ItemsType>
    {
        public virtual List<string> Serialize(List<KeyValuePair<string, ItemsType>> items)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < items.Count; i++)
            {
                result.AddRange(SerializeItem(items[i]));
            }

            return result;
        }

        protected abstract List<string> SerializeItem(KeyValuePair<string, ItemsType> item);
    }
}
