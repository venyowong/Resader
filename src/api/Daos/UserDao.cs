using Resader.Api.Extensions;
using Resader.Common.Entities;
using Resader.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Resader.Api.Daos
{
    public class UserDao
    {
        private IDbConnection connection;

        static UserDao()
        {
            Utility.MakeDapperMapping(typeof(User));
        }

        public UserDao(IDbConnection connection)
        {
            this.connection = connection;
        }

        public async Task<User> GetUserByMail(string mail)
        {
            if (string.IsNullOrWhiteSpace(mail))
            {
                return null;
            }

            return await connection.QueryFirstOrDefaultWithPolly<User>("SELECT * FROM user WHERE mail=@mail;", new { mail });
        }

        public async Task<User> GetUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            return await connection.QueryFirstOrDefaultWithPolly<User>("SELECT * FROM user WHERE id=@id;", new { id });
        }

        public async Task<bool> CreateUser(string id, string mail, string password)
        {
            if (string.IsNullOrWhiteSpace(mail) || string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            var salt = Guid.NewGuid().ToString("N");
            return await connection.ExecuteWithPolly("INSERT INTO user(id, mail, password, salt, create_time, update_time) VALUES(@id, @mail, @password, @salt, now(), now());", new
            {
                id,
                mail,
                salt,
                password = $"{password}{salt}".Md5()
            });
        }

        public async Task<bool> UpdateUser(User user)
        {
            if (user == null)
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

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await this.connection.QueryWithPolly<User>("SELECT * FROM user");
        }
    }
}