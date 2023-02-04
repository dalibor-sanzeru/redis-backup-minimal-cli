using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RedisBackupMinimalCli.Serialization
{
    public class RedisTypeDeserializer : IRedisTypeDeserializer
    {
        public const string KeyTypeString = "SET";

        public RedisType GetRedisType(string command)
        {
            int indexOfCommandSplitter = command.IndexOf(' ');

            if (indexOfCommandSplitter < 0)
            {
                return RedisType.Unknown;
            }
            string commandRedis = command.Substring(0, indexOfCommandSplitter).ToUpper();

            switch (commandRedis)
            {
                case KeyTypeString:
                    return RedisType.String;
                case "HSET":
                    return RedisType.Hash;
                case "SADD":
                    return RedisType.Set;
                case "ZADD":
                    return RedisType.SortedSet;
                case "LPUSH":
                    return RedisType.List;
                case "XADD":
                    return RedisType.Stream;
                default:
                    return RedisType.Unknown;
            }
        }

        public KeyValuePair<string, RedisValue> DeSerializeString(string command)
        {
            string commandKeysAndValuesOnly = RemoveCommandString(command, KeyTypeString);
            var (key, restOfCommand) = this.ExtractCommandFirstKey(commandKeysAndValuesOnly);

            return new KeyValuePair<string, RedisValue>(key, restOfCommand);
        }

        private string RemoveCommandString(string command, string commandKey)
        {
            return command.Trim().Remove(0, commandKey.Length).Trim();
        }

        private (string key, string restOfCommand) ExtractCommandFirstKey(string command)
        {
            int closeIndex = 0;
            for (int i = 1; i < command.Length; i++)
            {
                if (command[i] == '\"')
                {
                    closeIndex = i;
                    break;
                }
            }

            string key = command.Substring(1, closeIndex - 1);
            string value = command.Substring(closeIndex + 1).Trim();

            value = value.Remove(value.Length - 1).Remove(0, 1);

            return (key, value);
        }
    }
}
