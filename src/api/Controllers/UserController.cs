using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Resader.Api.Daos;
using Resader.Common.Extensions;
using Resader.Api.Services;
using Resader.Api.Attributes;
using Resader.Common.Api.Response;
using Resader.Common.Api.Request;
using Resader.Common.Entities;

namespace Resader.Api.Controllers
{
    [ApiController]
    [Route("/User")]
    public class UserController : BaseController
    {
        private UserService service;

        public UserController(UserService service)
        {
            this.service = service;
        }

        [HttpPost("Login")]
        public async Task<Result<UserResponse>> Login([Required] LoginRequest request)
        {
            var user = await this.service.GetUserByMail(request.Mail);
            if (user == null)
            {
                return Result<UserResponse>.Fail(1, "用户不存在");
            }

            if (user.Password != $"{request.Password}{user.Salt}".Md5())
            {
                return Result<UserResponse>.Fail(1, "密码错误");
            }

            return Result.Success(this.GetToken(user));
        }

        [JwtValidation]
        [HttpPost("ResetPassword")]
        public async Task<Result> ResetPassword([Required] ResetPasswordRequest request)
        {
            var user = await this.service.GetUser(this.GetUserId());
            if (user == null)
            {
                return Result.Fail(1, "用户不存在");
            }

            if (user.Password != $"{request.OldPassword}{user.Salt}".Md5())
            {
                return Result.Fail(1, "原始密码验证失败");
            }

            user.Password = $"{request.Password}{user.Salt}".Md5();
            if (await this.service.UpdateUser(user))
            {
                return Result.Success();
            }
            else
            {
                return Result.Fail(2);
            }
        }

        [HttpPost("SignUp")]
        public async Task<Result<UserResponse>> SignUp([Required] SignUpRequest request, [FromServices] UserDao dao)
        {
            var user = await this.service.GetUserByMail(request.Mail);
            if (user != null)
            {
                return Result<UserResponse>.Fail(101);
            }

            var id = Guid.NewGuid().ToString("N");
            if (await dao.CreateUser(id, request.Mail, request.Password))
            {
                return Result.Success(this.GetToken(new User
                {
                    Id = id,
                    Mail = request.Mail
                }));
            }

            return Result<UserResponse>.Fail(2);
        }

        private UserResponse GetToken(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                Mail = user.Mail,
                Token = JwtService.GetToken(user),
                Role = user.Role
            };
        }
    }
}
