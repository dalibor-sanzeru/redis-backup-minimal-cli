using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RedisBackupMinimalCli.Serialization
{
    public class RedisTypeDeserializer : IRedisTypeDeserializer
    {
        public const string KeyTypeRedisCommand = "SET";
        public const string HashTypeRedisCommand = "HSET";
        public const string SetTypeRedisCommand = "SADD";
        public const string ListTypeRedisCommand = "RPUSH";
        public const string SortedSetRedisCommand = "ZADD";
        public const string StreamRedisCommand = "XADD";

        private string RemoveCommandString(string command, string commandKey)
        {
            return command.Trim().Remove(0, commandKey.Length).Trim();
        }

        private (string key, string restOfCommand) ExtractKeyDelimited(string command, char delimiter = '\"', int startFrom = 1)
        {
            //TODO: Replace for with linq
            //int closeIndex = command.Skip(startFrom).TakeWhile(c => c != delimiter).Count() + startFrom;

            int closeIndex = 0;
            for (int i = startFrom; i < command.Length; i++)
            {
                if (command[i] == delimiter)
                {
                    closeIndex = i;
                    break;
                }
            }

            string key = command.Substring(startFrom, closeIndex - startFrom);
            string restOfcommand = command.Substring(closeIndex + 1).Trim();

            return (key, restOfcommand);
        }

        private string ExtractValueDelimited(string command)
        {
            var commandTrimmed = command.Trim();
            return commandTrimmed.Substring(1, commandTrimmed.Length - 2);
        }

        private string[] ExtractValuesDelimited(string command)
        {
            var commandTrimmed = command.Trim();

            Regex regex = new Regex("\"(\\s)*\""); // Split on occurence of empty space " "

            //Filter out empty strings
            string[] substrings = regex.Split(commandTrimmed)
                .Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();

            //Update first and last item if more than one is found
            if (substrings.Length > 0)
            {
                substrings[0] = substrings[0].TrimStart('"');
                substrings[substrings.Length - 1] = substrings[substrings.Length - 1].TrimEnd('"');
            }

            return substrings;
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
            var (key, restOfCommand) = this.ExtractKeyDelimited(commandKeysAndValuesOnly);
            var value = this.ExtractValueDelimited(restOfCommand);

            return new KeyValuePair<string, RedisValue>(key, value);
        }

        public KeyValuePair<string, HashEntry> DeSerializeHash(string command)
        {
            string commandKeysAndValuesOnly = RemoveCommandString(command, HashTypeRedisCommand);

            var (key, commandLeft1) = ExtractKeyDelimited(commandKeysAndValuesOnly);
            var (subkey, commandLeft2) = ExtractKeyDelimited(commandLeft1);
            var value = ExtractValueDelimited(commandLeft2);

            return new KeyValuePair<string, HashEntry>(key, new HashEntry(subkey, value));
        }

        public KeyValuePair<string, RedisValue> DeSerializeSet(string command)
        {
            string commandKeysAndValuesOnly = RemoveCommandString(command, SetTypeRedisCommand);

            var (key, commandLeft1) = ExtractKeyDelimited(commandKeysAndValuesOnly);
            var value = ExtractValueDelimited(commandLeft1);

            return new KeyValuePair<string, RedisValue>(key, value);
        }

        public KeyValuePair<string, RedisValue[]> DeSerializeList(string command)
        {
            string commandKeysAndValuesOnly = RemoveCommandString(command, ListTypeRedisCommand);

            var (key, commandLeft1) = ExtractKeyDelimited(commandKeysAndValuesOnly);
            var value = ExtractValuesDelimited(commandLeft1);

            return new KeyValuePair<string, RedisValue[]>(key, value.Select(x => new RedisValue(x)).ToArray());
        }

        public KeyValuePair<string, SortedSetEntry> DeSerializeSortedSet(string command)
        {
            string commandKeysAndValuesOnly = RemoveCommandString(command, SortedSetRedisCommand);

            var (key, commandLeft1) = ExtractKeyDelimited(commandKeysAndValuesOnly);
            var (score, commandLeft2) = ExtractKeyDelimited(commandLeft1, ' ', 0);
            var value = ExtractValueDelimited(commandLeft2);

            return new KeyValuePair<string, SortedSetEntry>(key, new SortedSetEntry(value, double.Parse(score)));
        }

        public KeyValuePair<string, StreamEntry> DeSerializeStream(string command)
        {
            throw new NotImplementedException();
        }
    }
}
