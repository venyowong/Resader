using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Resader.Grains.Models;

namespace Resader.Host
{
    static class Utility
    {
        public static string GetMd5Hash(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            using (MD5 md5Hash = MD5.Create())
            {
                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Create a new Stringbuilder to collect the bytes
                // and create a string.
                StringBuilder sBuilder = new StringBuilder();

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

        public static Task<Result<T>> ToResult<T>(this T t, int code = 0, string message = null)
        {
            return Task.FromResult(new Result<T>
            {
                Code = code,
                Data = t,
                Message = message
            });
        }

        public static Task<Result> ToResult(this int code, string message)
        {
            return Task.FromResult(new Result
            {
                Code = code,
                Message = message
            });
        }

        public static void MakeDapperMapping(string namspace)
        {
            foreach (var type in Assembly.GetEntryAssembly().GetTypes().Where(t => t.FullName.Contains(namspace)))
            {
                var map = new CustomPropertyTypeMap(type, (t, columnName) => t.GetProperties().FirstOrDefault(
                    prop => GetDescriptionFromAttribute(prop) == columnName || prop.Name.ToLower().Equals(columnName.ToLower())));
                Dapper.SqlMapper.SetTypeMap(type, map);
            }
        }

        private static string GetDescriptionFromAttribute(MemberInfo member)
        {
            if (member == null) return null;

            var attrib = (DescriptionAttribute)Attribute.GetCustomAttribute(member, typeof(DescriptionAttribute), false);
            return attrib == null ? null : attrib.Description;
        }
    }
}