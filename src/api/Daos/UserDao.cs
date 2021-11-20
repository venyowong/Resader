using Resader.Api.Extensions;
using Resader.Api.Factories;
using Resader.Common.Entities;
using Resader.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Resader.Api.Daos;
public class UserDao
{
    private DbConnectionFactory connectionFactory;

    static UserDao()
    {
        Utility.MakeDapperMapping(typeof(User));
    }

    public UserDao(DbConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public async Task<User> GetUserByMail(string mail)
    {
        if (string.IsNullOrWhiteSpace(mail))
        {
            return null;
        }

        using var connection = await connectionFactory.Create();
        return await connection.QueryFirstOrDefaultWithPolly<User>("SELECT * FROM user WHERE mail=@mail;", new { mail });
    }

    public async Task<User> GetUser(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return null;
        }

        using var connection = await connectionFactory.Create();
        return await connection.QueryFirstOrDefaultWithPolly<User>("SELECT * FROM user WHERE id=@id;", new { id });
    }

    public async Task<bool> CreateUser(string id, string mail, string password)
    {
        if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(mail) || string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        using var connection = await connectionFactory.Create();
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

        using var connection = await connectionFactory.Create();
        return await connection.ExecuteWithPolly("UPDATE user SET password=@Password, update_time=now() WHERE id=@Id OR mail=@Mail;", new
        {
            user.Id,
            user.Mail,
            user.Password
        });
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
        using var connection = await connectionFactory.Create();
        return await connection.QueryWithPolly<User>("SELECT * FROM user");
    }

    public async Task<bool> CreateUser(User user)
    {
        if (user == null)
        {
            return false;
        }

        using var connection = await connectionFactory.Create();
        var salt = Guid.NewGuid().ToString("N");
        var password = Guid.NewGuid().ToString("N").Md5();
        return await connection.ExecuteWithPolly(@"INSERT INTO user(id, mail, password, salt, create_time, update_time, oauth_id, name,
                avatar_url, source, url, location, company, blog, bio) VALUES(@Id, @Mail, @password, @salt, now(), now(), @OauthId, @Name,
                @AvatarUrl, @Source, @Url, @Location, @Company, @Blog, @Bio);", new
        {
            user.Id,
            user.Mail,
            salt,
            password = $"{password}{salt}".Md5(),
            user.OauthId,
            user.Name,
            user.AvatarUrl,
            user.Source,
            user.Url,
            user.Location,
            user.Company,
            user.Blog,
            user.Bio
        });
    }
}
