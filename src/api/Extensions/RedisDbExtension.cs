using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Polly;
using StackExchange.Redis;

namespace Resader.Api.Extensions
{
    public static class RedisDbExtension
    {
        private static ConcurrentDictionary<string, object> _dictionary = new ConcurrentDictionary<string, object>();

        public static async Task<bool> StringSetWithPolly(this IDatabase db, RedisKey redisKey, RedisValue value, 
            TimeSpan? expiry = null, When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            var key = $"{db.Database}StringSet";
            IAsyncPolicy<bool> policy = null;
            if (_dictionary.ContainsKey(key))
            {
                policy = _dictionary[key] as IAsyncPolicy<bool>;
            }
            else
            {
                policy = PollyPolicies.GetRedisCommandPolicy<bool>();
                _dictionary.TryAdd(key, policy);
            }

            return await policy.ExecuteAsync(() =>
            {
                return Task.FromResult(db.StringSet(redisKey, value, expiry, when, flags));
            });
        }

        public static async Task<RedisValue> StringGetWithPolly(this IDatabase db, RedisKey redisKey, CommandFlags flags = CommandFlags.None)
        {
            var key = $"{db.Database}StringGet.RedisValue";
            IAsyncPolicy<RedisValue> policy = null;
            if (_dictionary.ContainsKey(key))
            {
                policy = _dictionary[key] as IAsyncPolicy<RedisValue>;
            }
            else
            {
                policy = PollyPolicies.GetRedisCommandPolicy<RedisValue>();
                _dictionary.TryAdd(key, policy);
            }

            return await policy.ExecuteAsync(async () =>
            {
                return await db.StringGetAsync(redisKey, flags);
            });
        }

        public static async Task<RedisValue[]> HashGetWithPolly(this IDatabase db, RedisKey redisKey, RedisValue[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            var key = $"{db.Database}HashGet.RedisValue[]";
            IAsyncPolicy<RedisValue[]> policy = null;
            if (_dictionary.ContainsKey(key))
            {
                policy = _dictionary[key] as IAsyncPolicy<RedisValue[]>;
            }
            else
            {
                policy = PollyPolicies.GetRedisCommandPolicy<RedisValue[]>();
                _dictionary.TryAdd(key, policy);
            }

            return await policy.ExecuteAsync(() =>
            {
                return Task.FromResult(db.HashGet(redisKey, hashFields, flags));
            });
        }

        public static async Task<RedisValue> HashGetWithPolly(this IDatabase db, RedisKey redisKey, RedisValue hashField, CommandFlags flags = CommandFlags.None)
        {
            var key = $"{db.Database}HashGet.RedisKey";
            IAsyncPolicy<RedisValue> policy = null;
            if (_dictionary.ContainsKey(key))
            {
                policy = _dictionary[key] as IAsyncPolicy<RedisValue>;
            }
            else
            {
                policy = PollyPolicies.GetRedisCommandPolicy<RedisValue>();
                _dictionary.TryAdd(key, policy);
            }

            return await policy.ExecuteAsync(() =>
            {
                return Task.FromResult(db.HashGet(redisKey, hashField, flags));
            });
        }

        public static async Task<HashEntry[]> HashGetAllWithPolly(this IDatabase db, RedisKey redisKey, CommandFlags flags = CommandFlags.None)
        {
            var key = $"{db.Database}HashGetAll.HashEntry[]";
            IAsyncPolicy<HashEntry[]> policy = null;
            if (_dictionary.ContainsKey(key))
            {
                policy = _dictionary[key] as IAsyncPolicy<HashEntry[]>;
            }
            else
            {
                policy = PollyPolicies.GetRedisCommandPolicy<HashEntry[]>();
                _dictionary.TryAdd(key, policy);
            }

            return await policy.ExecuteAsync(() =>
            {
                return Task.FromResult(db.HashGetAll(redisKey, flags));
            });
        }

        public static async Task<bool> HashSetWithPolly(this IDatabase db, RedisKey redisKey, HashEntry[] hashFields, CommandFlags flags = CommandFlags.None)
        {
            var key = $"{db.Database}HashSet.HashEntry[]";
            IAsyncPolicy<bool> policy = null;
            if (_dictionary.ContainsKey(key))
            {
                policy = _dictionary[key] as IAsyncPolicy<bool>;
            }
            else
            {
                policy = PollyPolicies.GetRedisCommandPolicy<bool>();
                _dictionary.TryAdd(key, policy);
            }

            return await policy.ExecuteAsync(() =>
            {
                db.HashSet(redisKey, hashFields, flags);
                return Task.FromResult(true);
            });
        }

        public static async Task<bool> HashSetWithPolly(this IDatabase db, RedisKey redisKey, RedisValue hashField, RedisValue value, 
            When when = When.Always, CommandFlags flags = CommandFlags.None)
        {
            var key = $"{db.Database}HashSet.RedisKey";
            IAsyncPolicy<bool> policy = null;
            if (_dictionary.ContainsKey(key))
            {
                policy = _dictionary[key] as IAsyncPolicy<bool>;
            }
            else
            {
                policy = PollyPolicies.GetRedisCommandPolicy<bool>();
                _dictionary.TryAdd(key, policy);
            }

            return await policy.ExecuteAsync(() =>
            {
                return Task.FromResult(db.HashSet(redisKey, hashField, value, when, flags));
            });
        }

        public static async Task<bool> KeyDeleteWithPolly(this IDatabase db, RedisKey redisKey, CommandFlags flags = CommandFlags.None)
        {
            var key = $"{db.Database}KeyDelete";
            IAsyncPolicy<bool> policy = null;
            if (_dictionary.ContainsKey(key))
            {
                policy = _dictionary[key] as IAsyncPolicy<bool>;
            }
            else
            {
                policy = PollyPolicies.GetRedisCommandPolicy<bool>();
                _dictionary.TryAdd(key, policy);
            }

            return await policy.ExecuteAsync(() => db.KeyDeleteAsync(redisKey, flags));
        }
    }
}