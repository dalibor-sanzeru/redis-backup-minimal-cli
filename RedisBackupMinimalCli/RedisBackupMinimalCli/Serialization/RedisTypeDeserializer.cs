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
        public const string HashTypeString = "HSET";

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
            var (key, restOfCommand) = this.ExtractDelimited(commandKeysAndValuesOnly);
            var (value, _) = this.ExtractDelimited(restOfCommand);

            return new KeyValuePair<string, RedisValue>(key, value);
        }

        public KeyValuePair<string, HashEntry> DeSerializeHash(string command)
        {
            string commandKeysAndValuesOnly = RemoveCommandString(command, HashTypeString);

            var (key, commandLeft1) = ExtractDelimited(commandKeysAndValuesOnly);
            var (subkey, commandLeft2) = ExtractDelimited(commandLeft1);
            var (value, _) = ExtractDelimited(commandLeft2);

            return new KeyValuePair<string, HashEntry>(key, new HashEntry(subkey, value));
        }

        private string RemoveCommandString(string command, string commandKey)
        {
            return command.Trim().Remove(0, commandKey.Length).Trim();
        }

        private (string key, string restOfCommand) ExtractDelimited(string command)
        {
            var commandTrimmed = command;

            int closeIndex = 0;
            for (int i = 1; i < commandTrimmed.Length; i++)
            {
                if (command[i] == '\"')
                {
                    closeIndex = i;
                    break;
                }
            }

            string key = commandTrimmed.Substring(1, closeIndex - 1);
            string restOfcommand = commandTrimmed.Substring(closeIndex + 1).Trim();

            return (key, restOfcommand);
        }
    }
}
