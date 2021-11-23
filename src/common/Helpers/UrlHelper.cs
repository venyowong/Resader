using System;
using System.Collections.Generic;
using System.Text;

namespace Resader.Common.Helpers
{
    public static class UrlHelper
    {
        public static string AddParamIntoQuery(string url, string key, string value)
        {
            if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(key))
            {
                return url;
            }

            var index1 = url.IndexOf('?');
            var index2 = url.IndexOf('#');
            if (index1 < 0)
            {
                if (index2 < 0)
                {
                    return $"{url}?{GetKeyValuePairString(key, value)}";
                }

                var path = url.Substring(0, index2);
                return $"{path}?{GetKeyValuePairString(key, value)}{url.Substring(index2)}";
            }

            if (index2 < 0)
            {
                if (index1 == url.Length - 1)
                {
                    return $"{url}{GetKeyValuePairString(key, value)}";
                }

                return $"{url}&{GetKeyValuePairString(key, value)}";
            }

            if (index1 == index2 - 1)
            {
                var path = url.Substring(0, index2);
                return $"{path}{GetKeyValuePairString(key, value)}{url.Substring(index2)}";
            }
            else
            {
                var path = url.Substring(0, index2);
                return $"{path}&{GetKeyValuePairString(key, value)}{url.Substring(index2)}";
            }
        }

        private static string GetKeyValuePairString(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return key;
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                return string.Empty;
            }
            return $"{key}={value}";
        }
    }
}
