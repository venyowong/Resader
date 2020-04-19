using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Orleans;
using Orleans.Concurrency;
using Orleans.Http.Abstractions;
using Resader.Grains;
using Resader.Grains.Models;
using Resader.Host.Daos;

namespace Resader.Host.Grains
{
    [StatelessWorker]
    public class UserGrain : Grain, IUserGrain
    {
        private IDbConnection connection;

        public Configuration config;

        public UserGrain(IDbConnection connection, IOptions<Configuration> config)
        {
            this.connection = connection;
            this.config = config?.Value;
        }

        public Task<Result<User>> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Mail) || string.IsNullOrWhiteSpace(request?.Password))
            {
                return default(User).ToResult(1);
            }

            using (this.connection)
            {
                var user = this.connection.GetUserByMail(request.Mail);
                if (user == null)
                {
                    return default(User).ToResult(101);
                }

                if (user.Password != $"{request.Password}{user.Salt}".GetMd5Hash())
                {
                    return default(User).ToResult(102);
                }

                return this.GetToken(new Resader.Grains.Models.User
                {
                    Id = user.Id,
                    Mail = user.Mail
                }).ToResult();
            }
        }

        public Task<Result> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Password) || string.IsNullOrWhiteSpace(request?.OldPassword))
            {
                return 1.ToResult(string.Empty);
            }

            var userId = this.GetPrimaryKeyString();
            using (this.connection)
            {
                var user = this.connection.GetUser(userId);
                if (user == null)
                {
                    return 1.ToResult("userId 对应的用户不存在");
                }

                if (user.Password != $"{request.OldPassword}{user.Salt}".GetMd5Hash())
                {
                    return 1.ToResult("原始密码验证失败");
                }

                user.Password = $"{request.Password}{user.Salt}".GetMd5Hash();
                if (this.connection.UpdateUser(user) > 0)
                {
                    return 0.ToResult(string.Empty);
                }
                else
                {
                    return 2.ToResult(string.Empty);
                }
            }
        }

        public Task<Result<User>> SignUp([FromBody] SignUpRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Mail) || string.IsNullOrWhiteSpace(request?.Password))
            {
                return default(User).ToResult(1);
            }

            using (this.connection)
            {
                var user = this.connection.GetUserByMail(request.Mail);
                if (user != null)
                {
                    return default(User).ToResult(101);
                }

                var id = Guid.NewGuid().ToString("N");
                if (this.connection.CreateUser(id, request.Mail, request.Password) > 0)
                {
                    return this.GetToken(new User
                    {
                        Id = id,
                        Mail = request.Mail
                    }).ToResult();
                }

                return default(User).ToResult(2);
            }
        }

        private User GetToken(User user)
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
            user.Token = tokenHandler.WriteToken(token);
            return user;
        }
    }
}