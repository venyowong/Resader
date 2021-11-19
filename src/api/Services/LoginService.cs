using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OpenSecurity.Oauth;
using Resader.Api;
using Resader.Api.Daos;
using Resader.Api.Services;
using Resader.Common.Entities;
using Resader.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Resader.Services
{
    public class LoginService : ILogin
    {
        private Configuration config;
        private UserDao userDao;
        private UserService userService;

        public LoginService(IOptions<Configuration> config, UserDao userDao, UserService userService)
        {
            this.config = config.Value;
            this.userDao = userDao;
            this.userService = userService;
        }

        public async Task Login(HttpContext context, UserInfo user)
        {
            if (string.IsNullOrWhiteSpace(user.Mail))
            {
                context.Response.Redirect($"{this.config.OauthLoginUrl}?code=1&msg={WebUtility.UrlEncode("三方帐号未关联邮箱")}");
                return;
            }

            var userInfo = await this.userService.GetUserByMail(user.Mail);
            if (userInfo != null)
            {
                var session = this.userService.GetTokenSession(userInfo);
                context.Response.Redirect($"{this.config.OauthLoginUrl}?code=0&session={session}");
                return;
            }

            userInfo = new User
            {
                Id = Guid.NewGuid().ToString("N"),
                Mail = user.Mail,
                OauthId = user.Id,
                Name = user.Name,
                AvatarUrl = user.AvatarUrl,
                Source = user.Source,
                Url = user.Url,
                Location = user.Location,
                Company = user.Company,
                Blog = user.Blog,
                Bio = user.Bio
            };
            if (await this.userDao.CreateUser(userInfo))
            {
                var session = this.userService.GetTokenSession(userInfo);
                context.Response.Redirect($"{this.config.OauthLoginUrl}?code=0&session={session}");
                return;
            }

            context.Response.Redirect($"{this.config.OauthLoginUrl}?code=-1");
            return;
        }
    }
}
