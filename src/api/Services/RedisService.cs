using Resader.Api.Extensions;
using Resader.Api.Helpers;
using Resader.Common.Extensions;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resader.Api.Services
{
    public class RedisService : ICacheService
    {
        private IDatabase db;

        public RedisService(IDatabase db)
        {
            this.db = db;
        }

        public bool DeleteKey(string key)
        {
            return AsyncHelper.RunSync(() => this.db.KeyDeleteWithPolly(key));
        }

        public T GetWithInit<T>(string key, Func<T> init, TimeSpan? expiry = null) where T : class
        {
            var json = this.db.StringGetWithPolly(key).Result;
            if (!json.IsNullOrEmpty)
            {
                return json.ToString().ToObj<T>();
            }

            var value = init();
            if (value == null)
            {
                return value;
            }

            this.db.StringSetWithPolly(key, value.ToJson(), expiry).Wait();
            return value;
        }

        public void HashDelete(string key, string[] hashFields)
        {
            this.db.HashDeleteWithPolly(key, hashFields.Select(x => new RedisValue(x)).ToArray()).Wait();
        }

        public void HashDelete(string key, string hashField)
        {
            this.db.HashDeleteWithPolly(key, hashField).Wait();
        }

        public Dictionary<string, string> HashGet(string key, string[] hashFields)
        {
            var values = AsyncHelper.RunSync(() => this.db.HashGetWithPolly(key, hashFields.Select(x => new RedisValue(x)).ToArray()));
            var result = new Dictionary<string, string>();
            for (int i = 0; i < hashFields.Length; i++)
            {
                if (!values[i].HasValue || values[i].IsNullOrEmpty)
                {
                    continue;
                }

                result.Add(hashFields[i], values[i]);
            }
            return result;
        }

        public string HashGet(string key, string hashField)
        {
            return AsyncHelper.RunSync(() => this.db.HashGetWithPolly(key, hashField));
        }

        public Dictionary<string, string> HashGetAll(string key)
        {
            var entries = AsyncHelper.RunSync(() => this.db.HashGetAllWithPolly(key));
            if (!entries.Any())
            {
                return new Dictionary<string, string>();
            }

            return entries.ToDictionary(x => x.Name.ToString(), x => x.Value.ToString());
        }

        public void HashSet(string key, Dictionary<string, string> hashFields)
        {
            AsyncHelper.RunSync(() => this.db.HashSetWithPolly(key, hashFields.Select(x => new HashEntry(x.Key, x.Value)).ToArray()));
        }

        public void HashSet(string key, string hashField, string value)
        {
            AsyncHelper.RunSync(() => this.db.HashSetWithPolly(key, hashField, value));
        }

        public string StringGet(string key)
        {
            return AsyncHelper.RunSync(() => this.db.StringGetWithPolly(key));
        }

        public void StringSet(string key, string value, TimeSpan? expiry = null)
        {
            AsyncHelper.RunSync(() => this.db.StringSetWithPolly(key, value, expiry));
        }
    }
}
