using System;
using System.Data;
using System.Threading.Tasks;
using Resader.Extensions;
using Resader.Models;

namespace Resader.Daos
{
    public static class UserDao
    {
        public static async Task<User> GetUserByMail(this IDbConnection connection, string mail)
        {
            if (string.IsNullOrWhiteSpace(mail) || connection == null)
            {
                return null;
            }

            return await connection.QueryFirstOrDefaultWithPolly<User>("SELECT * FROM user WHERE mail=@mail;", new { mail });
        }

        public static async Task<User> GetUser(this IDbConnection connection, string id)
        {
            if (string.IsNullOrWhiteSpace(id) || connection == null)
            {
                return null;
            }

            return await connection.QueryFirstOrDefaultWithPolly<User>("SELECT * FROM user WHERE id=@id;", new { id });
        }

        public static async Task<bool> CreateUser(this IDbConnection connection, string id, string mail, string password)
        {
            if (string.IsNullOrWhiteSpace(mail) || connection == null || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            var salt = Guid.NewGuid().ToString("N");
            return await connection.ExecuteWithPolly("INSERT INTO user(id, mail, password, salt, create_time, update_time) VALUES(@id, @mail, @password, @salt, now(), now());", new
            {
                id,
                mail,
                salt,
                password = $"{password}{salt}".GetMd5Hash()
            });
        }

        public static async Task<bool> UpdateUser(this IDbConnection connection, User user)
        {
            if (connection == null || user == null)
            {
                return false;
            }

            return await connection.ExecuteWithPolly("UPDATE user SET password=@Password, update_time=now() WHERE id=@Id OR mail=@Mail;", new
            {
                user.Id,
                user.Mail,
                user.Password
            });
        }
    }
}