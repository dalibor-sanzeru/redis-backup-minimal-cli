﻿using StackExchange.Redis;
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

        KeyValuePair<string, RedisValue> DeSerializeString(string command);

        KeyValuePair<string, HashEntry> DeSerializeHash(string command);
    }
}
