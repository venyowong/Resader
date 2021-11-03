using Resader.Api.Daos;
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
    }
}
