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

        public static string GZipCompress(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            using var outputStream = new MemoryStream();
            using var gZipStream = new GZipStream(outputStream, CompressionMode.Compress);
            gZipStream.Write(inputBytes, 0, inputBytes.Length);
            var outputBytes = outputStream.ToArray();
            return Convert.ToBase64String(outputBytes);
        }

        public static string GZipDecompress(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            byte[] inputBytes = Convert.FromBase64String(input);
            using var inputStream = new MemoryStream(inputBytes);
            using var gZipStream = new GZipStream(inputStream, CompressionMode.Decompress);
            using var streamReader = new StreamReader(gZipStream);
            return streamReader.ReadToEnd();
        }
    }
}
