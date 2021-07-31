using Microsoft.Extensions.Caching.Memory;
using Resader.Api.Helpers;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Resader.Api.Services
{
    public class MemoryCacheService : ICacheService
    {
        private IMemoryCache memoryCache;

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public bool DeleteKey(string key)
        {
            this.memoryCache.Remove(key);
            return true;
        }

        public void HashDelete(string key, string[] hashFields)
        {
            LockHelper.Lock($"MomeryCacheService.{key}", () =>
            {
                var map = this.HashGetAll(key);
                foreach (var field in hashFields)
                {
                    if (map.ContainsKey(field))
                    {
                        map.Remove(field);
                    }
                }
                this.memoryCache.Set(key, map);
            });
        }

        public void HashDelete(string key, string hashField)
        {
            LockHelper.Lock($"MomeryCacheService.{key}", () =>
            {
                var map = this.HashGetAll(key);
                if (map.ContainsKey(hashField))
                {
                    map.Remove(hashField);
                }
                this.memoryCache.Set(key, map);
            });
        }

        public Dictionary<string, string> HashGet(string key, string[] hashFields)
        {
            var map = this.HashGetAll(key);
            return map.Where(x => hashFields.Contains(x.Key))
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public string HashGet(string key, string hashField)
        {
            var map = this.HashGetAll(key);
            if (map.ContainsKey(hashField))
            {
                return map[hashField];
            }
            else
            {
                return string.Empty;
            }
        }

        public Dictionary<string, string> HashGetAll(string key)
        {
            var result = this.memoryCache.Get(key);
            if (result == null || !(result is Dictionary<string, string>))
            {
                return new Dictionary<string, string>();
            }
            return result as Dictionary<string, string>;
        }

        public void HashSet(string key, Dictionary<string, string> hashFields)
        {
            LockHelper.Lock($"MomeryCacheService.{key}", () =>
            {
                var cache = this.memoryCache.Get(key);
                if (cache == null)
                {
                    this.memoryCache.Set(key, hashFields);
                    return;
                }

                var map = cache as Dictionary<string, string>;
                if (map == null)
                {
                    this.memoryCache.Set(key, hashFields);
                    return;
                }

                foreach (var item in hashFields)
                {
                    if (!map.ContainsKey(item.Key))
                    {
                        map.Add(item.Key, item.Value);
                    }
                }
                this.memoryCache.Set(key, map);
            });
        }

        public void HashSet(string key, string hashField, string value)
        {
            LockHelper.Lock($"MomeryCacheService.{key}", () =>
            {
                var cache = this.memoryCache.Get(key);
                if (cache == null)
                {
                    this.memoryCache.Set(key, new Dictionary<string, string> { { hashField, value } });
                    return;
                }

                var map = cache as Dictionary<string, string>;
                if (map == null)
                {
                    this.memoryCache.Set(key, new Dictionary<string, string> { { hashField, value } });
                    return;
                }

                map.Add(hashField, value);
                this.memoryCache.Set(key, map);
            });
        }

        public string StringGet(string key)
        {
            return this.memoryCache.Get(key)?.ToString();
        }

        public void StringSet(string key, string value, TimeSpan? expiry = null)
        {
            if (expiry == null || expiry.Value.TotalSeconds <= 0)
            {
                this.memoryCache.Set(key, value);
            }
            else
            {
                this.memoryCache.Set(key, value, expiry.Value);
            }
        }

        public T GetWithInit<T>(string key, Func<T> init, TimeSpan? expiry = null) where T : class
        {
            var cache = this.memoryCache.Get<T>(key);
            if (cache != null)
            {
                return cache;
            }

            cache = init();
            if (cache == null)
            {
                return cache;
            }

            if (expiry == null)
            {
                this.memoryCache.Set(key, cache);
            }
            else
            { 
                this.memoryCache.Set(key, cache, expiry.Value);
            }
            return cache;
        }
    }
}
