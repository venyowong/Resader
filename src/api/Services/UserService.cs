using Resader.Api.Daos;
using Resader.Common.Api.Response;
using Resader.Common.Entities;
using Resader.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resader.Api.Services
{
    public class UserService
    {
        private ICacheService cache;
        private UserDao dao;

        public UserService(ICacheService cache, UserDao dao)
        {
            this.cache = cache;
            this.dao = dao;
        }

        public async Task<User> GetUserByMail(string mail)
        {
            if (string.IsNullOrWhiteSpace(mail))
            {
                return null;
            }

            var user = this.cache.HashGet(Const.UserCache, mail).ToObj<User>();
            if (user != null)
            {
                return user;
            }

            user = await this.dao.GetUserByMail(mail);
            if (user == null)
            {
                return null;
            }

            this.cache.HashSet(Const.UserCache, mail, user.ToJson());
            return user;
        }

        public async Task<User> GetUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return null;
            }

            var user = this.cache.HashGet(Const.UserCache, id).ToObj<User>();
            if (user != null)
            {
                return user;
            }

            user = await this.dao.GetUser(id);
            if (user == null)
            {
                return null;
            }

            this.cache.HashSet(Const.UserCache, id, user.ToJson());
            return user;
        }

        public async Task<bool> UpdateUser(User user)
        {
            if (!await this.dao.UpdateUser(user))
            {
                return false;
            }

            this.cache.HashSet(Const.UserCache, user.Mail, user.ToJson());
            this.cache.HashSet(Const.UserCache, user.Id, user.ToJson());
            return true;
        }

        /// <summary>
        /// 获取用于换取 token 的 session
        /// <para>session 过期时长为 5 分钟</para>
        /// <para>且只能换取一次 token</para>
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string GetTokenSession(User user)
        {
            var token = this.GetToken(user);
            var session = Guid.NewGuid().ToString("N");
            this.cache.StringSet($"{Const.TokenSessionCache}{session}", token.ToJson(), new TimeSpan(0, 5, 0));
            return session;
        }

        public UserResponse GetTokenBySession(string session)
        {
            var key = $"{Const.TokenSessionCache}{session}";
            var json = this.cache.StringGet(key);
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            this.cache.DeleteKey(key);
            return json.ToObj<UserResponse>();
        }

        public UserResponse GetToken(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                Mail = user.Mail,
                Token = JwtService.GetToken(user),
                Role = user.Role,
                AvatarUrl = user.AvatarUrl,
                Source = user.Source,
                Url = user.Url,
                Location = user.Location,
                Company = user.Company,
                Blog = user.Blog,
                Bio = user.Bio
            };
        }
    }
}
