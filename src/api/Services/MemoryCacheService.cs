using Microsoft.Extensions.Caching.Memory;
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
            var mutex = new Mutex(true, $"MomeryCacheService.{key}");
            try
            {
                mutex.WaitOne();

                var cache = this.memoryCache.Get(key);
                if (cache == null)
                {
                    this.memoryCache.Set(key, hashFields);
                }
                var map = cache as Dictionary<string, string>;
                if (map == null)
                {
                    this.memoryCache.Set(key, hashFields);
                }
                else
                {
                    foreach (var item in hashFields)
                    {
                        if (!map.ContainsKey(item.Key))
                        {
                            map.Add(item.Key, item.Value);
                        }
                    }
                    this.memoryCache.Set(key, map);
                }
            }
            catch (Exception e)
            {
                mutex.ReleaseMutex();
                Log.Error(e, "MomeryCacheService.HashSet");
            }
        }

        public void HashSet(string key, string hashField, string value)
        {
            var mutex = new Mutex(true, $"MomeryCacheService.{key}");
            try
            {
                mutex.WaitOne();

                var cache = this.memoryCache.Get(key);
                if (cache == null)
                {
                    this.memoryCache.Set(key, new Dictionary<string, string> { { hashField, value } });
                }
                var map = cache as Dictionary<string, string>;
                if (map == null)
                {
                    this.memoryCache.Set(key, new Dictionary<string, string> { { hashField, value } });
                }
                else
                {
                    map.Add(hashField, value);
                    this.memoryCache.Set(key, map);
                }
            }
            catch (Exception e)
            {
                mutex.ReleaseMutex();
                Log.Error(e, "MomeryCacheService.HashSet");
            }
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
    }
}
