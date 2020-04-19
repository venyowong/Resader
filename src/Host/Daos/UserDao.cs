using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Resader.Host.Models;

namespace Resader.Host.Daos
{
    public static class UserDao
    {
        public static User GetUserByMail(this IDbConnection connection, string mail)
        {
            if (string.IsNullOrWhiteSpace(mail) || connection == null)
            {
                return null;
            }

            return connection.QueryFirstOrDefault<User>("SELECT * FROM user WHERE mail=@mail;", new { mail });
        }

        public static User GetUser(this IDbConnection connection, string id)
        {
            if (string.IsNullOrWhiteSpace(id) || connection == null)
            {
                return null;
            }

            return connection.QueryFirstOrDefault<User>("SELECT * FROM user WHERE id=@id;", new { id });
        }

        public static int CreateUser(this IDbConnection connection, string id, string mail, string password)
        {
            if (string.IsNullOrWhiteSpace(mail) || connection == null || string.IsNullOrWhiteSpace(password))
            {
                return 0;
            }

            var salt = Guid.NewGuid().ToString("N");
            return connection.Execute("INSERT INTO user(id, mail, password, salt, create_time, update_time) VALUES(@id, @mail, @password, @salt, now(), now());", new
            {
                id,
                mail,
                salt,
                password = $"{password}{salt}".GetMd5Hash()
            });
        }

        public static int UpdateUser(this IDbConnection connection, User user)
        {
            if (connection == null || user == null)
            {
                return 0;
            }

            return connection.Execute("UPDATE user SET password=@Password, update_time=now() WHERE id=@Id OR mail=@Mail;", new
            {
                user.Id,
                user.Mail,
                user.Password
            });
        }
    }
}