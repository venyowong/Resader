using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Resader.Common.Extensions
{
    public static class HttpExtension
    {
        public static async Task<T> ReadAsObj<T>(this HttpResponseMessage response)
        {
            if (response == null || !response.IsSuccessStatusCode || response.Content == null)
            {
                return default;
            }

            var json = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }

            return json.ToObj<T>();
        }
    }
}
