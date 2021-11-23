using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OpenSecurity.Oauth;
using Resader.Api;
using Resader.Api.Daos;
using Resader.Api.Services;
using Resader.Common.Entities;
using Resader.Common.Helpers;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Resader.Services;

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
            userInfo.AvatarUrl = user.AvatarUrl;
            userInfo.Bio = user.Bio;
            userInfo.Blog = user.Blog;
            userInfo.Company = user.Company;
            userInfo.Location = user.Location;
            userInfo.Name = user.Name;
            userInfo.OauthId = user.Id;
            userInfo.Source = user.Source;
            await this.userService.UpdateUser(userInfo);

            var session = this.userService.GetTokenSession(userInfo);
            var url = UrlHelper.AddParamIntoQuery(this.config.OauthLoginUrl, "code", "0");
            url = UrlHelper.AddParamIntoQuery(url, "session", session);
            context.Response.Redirect(url);
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
            var url = UrlHelper.AddParamIntoQuery(this.config.OauthLoginUrl, "code", "0");
            url = UrlHelper.AddParamIntoQuery(url, "session", session);
            context.Response.Redirect(url);
            return;
        }

        context.Response.Redirect($"{this.config.OauthLoginUrl}?code=-1");
        return;
    }
}
