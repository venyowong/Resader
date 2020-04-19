using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Http.Abstractions;
using Resader.Grains.Models;

namespace Resader.Grains
{
    public interface IUserGrain : IGrainWithStringKey
    {
        /// <summary>
        /// 注册
        /// <para>101 用户已存在</para>
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("user/signup", routeGrainProviderPolicy: nameof(RandomRouteGrainProvider))]
        Task<Result<User>> SignUp([FromBody] SignUpRequest request);

        /// <summary>
        /// 登录
        /// <para>101 用户不存在</para>
        /// <para>102 密码错误</para>
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost("user/login", routeGrainProviderPolicy: nameof(RandomRouteGrainProvider))]
        Task<Result<User>> Login([FromBody] LoginRequest request);

        [Authorize]
        [HttpPost("{grainId}/user/resetpassword")]
        Task<Result> ResetPassword([FromBody] ResetPasswordRequest request);
    }
}
