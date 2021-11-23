using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace Resader.Common.Extensions
{
    public static class StringExtension
    {
        public static string Md5(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            using (var md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                var data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                var sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data 
                // and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                // Return the hexadecimal string.
                return sBuilder.ToString();
            }
        }

        public static string ToJson(this object obj)
        {
            if (obj == null)
            {
                return string.Empty;
            }

            try
            {
                return JsonConvert.SerializeObject(obj);
            }
            catch
            {
                return string.Empty;
            }
        }

        public static T ToObj<T>(this string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch
            {
                return default;
            }
        }

        #region gzip
        /// <summary>
        /// GZip 解压
        /// </summary>
        /// <param name="zippedString"></param>
        /// <returns></returns>
        public static string GZipDecompress(this string zippedString)
        {
            if (string.IsNullOrWhiteSpace(zippedString))
            {
                return string.Empty;
            }

            try
            {
                return Encoding.UTF8.GetString(GZipDecompress(Convert.FromBase64String(zippedString)));
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// GZip 解压
        /// </summary>
        /// <param name="zippedData"></param>
        /// <returns></returns>
        public static byte[] GZipDecompress(byte[] zippedData)
        {
            try
            {
                using (var stream = new GZipStream(new MemoryStream(zippedData), CompressionMode.Decompress))
                {
                    const int size = 4096;
                    var buffer = new byte[size];
                    using (var memory = new MemoryStream())
                    {
                        int count = 0;
                        do
                        {
                            count = stream.Read(buffer, 0, size);
                            if (count > 0)
                            {
                                memory.Write(buffer, 0, count);
                            }
                        }
                        while (count > 0);
                        return memory.ToArray();
                    }
                }
            }
            catch
            {
                return new byte[0];
            }
        }

        public static string GZipCompress(this string rawString)
        {
            if (string.IsNullOrWhiteSpace(rawString))
            {
                return string.Empty;
            }

            try
            {
                return Convert.ToBase64String(GZipCompress(Encoding.UTF8.GetBytes(rawString)));
            }
            catch
            {
                return string.Empty;
            }
        }

        private static byte[] GZipCompress(byte[] rawData)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    var gzipStream = new GZipStream(memoryStream, CompressionMode.Compress, true);
                    gzipStream.Write(rawData, 0, rawData.Length);
                    gzipStream.Close();
                    return memoryStream.ToArray();
                }
            }
            catch
            {
                return new byte[0];
            }
        }
        #endregion
    }
}
