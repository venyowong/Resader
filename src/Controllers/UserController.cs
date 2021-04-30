using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Resader.Daos;
using Resader.Extensions;
using Resader.Models;
using Resader.Models.Request;
using Resader.Models.Response;

namespace Resader.Controllers
{
    [ApiController]
    [Route("/User")]
    public class UserController : Controller
    {
        private Configuration config;
        private UserDao dao;

        public UserController(UserDao dao, IOptions<Configuration> config)
        {
            this.dao = dao;
            this.config = config?.Value;
        }
        
        [HttpPost("Login")]
        public async Task<Result<UserResponse>> Login([Required] LoginRequest request)
        {
            var user = await this.dao.GetUserByMail(request.Mail);
            if (user == null)
            {
                return Result<UserResponse>.Fail(101);
            }

            if (user.Password != $"{request.Password}{user.Salt}".Md5())
            {
                return Result<UserResponse>.Fail(102);
            }

            return Result.Success(this.GetToken(user));
        }

        [Authorize]
        [HttpPost("ResetPassword")]
        public async Task<Result> ResetPassword([Required] ResetPasswordRequest request)
        {
            var user = await this.dao.GetUser(request.UserId);
            if (user == null)
            {
                return Result.Fail(1, "用户不存在");
            }

            if (user.Password != $"{request.OldPassword}{user.Salt}".Md5())
            {
                return Result.Fail(1, "原始密码验证失败");
            }

            user.Password = $"{request.Password}{user.Salt}".Md5();
            if (await this.dao.UpdateUser(user))
            {
                return Result.Success();
            }
            else
            {
                return Result.Fail(2);
            }
        }

        [HttpPost("SignUp")]
        public async Task<Result<UserResponse>> SignUp([Required] SignUpRequest request)
        {
            var user = this.dao.GetUserByMail(request.Mail);
            if (user != null)
            {
                return Result<UserResponse>.Fail(101);
            }

            var id = Guid.NewGuid().ToString("N");
            if (await this.dao.CreateUser(id, request.Mail, request.Password))
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
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this.config?.Jwt?.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Sid, user.Id),
                    new Claim(ClaimTypes.Email, user.Mail),
                    new Claim(ClaimTypes.Role, "user")
                }),
                Expires = DateTime.UtcNow.AddSeconds(this.config?.Jwt?.Expiration ?? 2592000),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new UserResponse
            {
                Id = user.Id,
                Mail = user.Mail,
                Token = tokenHandler.WriteToken(token)
            };
        }
    }
}