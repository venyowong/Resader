using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Polly;
using StackExchange.Redis;

namespace Resader.Extensions
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
    }
}