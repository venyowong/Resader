using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resader.Api.Services
{
    public interface ICacheService
    {
        void HashSet(string key, Dictionary<string, string> hashFields);

        void HashSet(string key, string hashField, string value);

        Dictionary<string, string> HashGetAll(string key);

        Dictionary<string, string> HashGet(string key, string[] hashFields);

        string HashGet(string key, string hashField);

        string StringGet(string key);

        void StringSet(string key, string value, TimeSpan? expiry = null);

        bool DeleteKey(string key);
    }
}
