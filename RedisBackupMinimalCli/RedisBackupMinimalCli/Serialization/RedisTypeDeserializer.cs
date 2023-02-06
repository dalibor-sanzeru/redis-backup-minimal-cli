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
        public const string KeyTypeRedisCommand = "SET";
        public const string HashTypeRedisCommand = "HSET";
        public const string SetTypeRedisCommand = "SADD";
        public const string ListTypeRedisCommand = "LPUSH";
        public const string SortedSetRedisCommand = "ZADD";
        public const string StreamRedisCommand = "XADD";

        private string RemoveCommandString(string command, string commandKey)
        {
            return command.Trim().Remove(0, commandKey.Length).Trim();
        }

        private (string key, string restOfCommand) ExtractDelimited(string command, char delimiter = '\"', int startFrom = 1)
        {
            var commandTrimmed = command;

            int closeIndex = 0;
            for (int i = startFrom; i < commandTrimmed.Length; i++)
            {
                if (command[i] == delimiter)
                {
                    closeIndex = i;
                    break;
                }
            }

            string key = commandTrimmed.Substring(startFrom, closeIndex - startFrom);
            string restOfcommand = commandTrimmed.Substring(closeIndex + 1).Trim();

            return (key, restOfcommand);
        }

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
                case KeyTypeRedisCommand:
                    return RedisType.String;
                case HashTypeRedisCommand:
                    return RedisType.Hash;
                case SetTypeRedisCommand:
                    return RedisType.Set;
                case SortedSetRedisCommand:
                    return RedisType.SortedSet;
                case ListTypeRedisCommand:
                    return RedisType.List;
                case StreamRedisCommand:
                    return RedisType.Stream;
                default:
                    return RedisType.Unknown;
            }
        }

        public KeyValuePair<string, RedisValue> DeSerializeString(string command)
        {
            string commandKeysAndValuesOnly = RemoveCommandString(command, KeyTypeRedisCommand);
            var (key, restOfCommand) = this.ExtractDelimited(commandKeysAndValuesOnly);
            var (value, _) = this.ExtractDelimited(restOfCommand);

            return new KeyValuePair<string, RedisValue>(key, value);
        }

        public KeyValuePair<string, HashEntry> DeSerializeHash(string command)
        {
            string commandKeysAndValuesOnly = RemoveCommandString(command, HashTypeRedisCommand);

            var (key, commandLeft1) = ExtractDelimited(commandKeysAndValuesOnly);
            var (subkey, commandLeft2) = ExtractDelimited(commandLeft1);
            var (value, _) = ExtractDelimited(commandLeft2);

            return new KeyValuePair<string, HashEntry>(key, new HashEntry(subkey, value));
        }

        public KeyValuePair<string, RedisValue> DeSerializeSet(string command)
        {
            string commandKeysAndValuesOnly = RemoveCommandString(command, SetTypeRedisCommand);

            var (key, commandLeft1) = ExtractDelimited(commandKeysAndValuesOnly);
            var (value, _) = ExtractDelimited(commandLeft1);

            return new KeyValuePair<string, RedisValue>(key, value);
        }

        public KeyValuePair<string, RedisValue> DeSerializeList(string command)
        {
            string commandKeysAndValuesOnly = RemoveCommandString(command, ListTypeRedisCommand);

            var (key, commandLeft1) = ExtractDelimited(commandKeysAndValuesOnly);
            var (value, _) = ExtractDelimited(commandLeft1);

            return new KeyValuePair<string, RedisValue>(key, value);
        }

        public KeyValuePair<string, SortedSetEntry> DeSerializeSortedSet(string command)
        {
            string commandKeysAndValuesOnly = RemoveCommandString(command, SortedSetRedisCommand);

            var (key, commandLeft1) = ExtractDelimited(commandKeysAndValuesOnly);
            var (score, commandLeft2) = ExtractDelimited(commandLeft1, ' ', 0);
            var (value, _) = ExtractDelimited(commandLeft2);

            return new KeyValuePair<string, SortedSetEntry>(key, new SortedSetEntry(value, double.Parse(score)));
        }

        public KeyValuePair<string, StreamEntry> DeSerializeStream(string command)
        {
            throw new NotImplementedException();
        }
    }
}
