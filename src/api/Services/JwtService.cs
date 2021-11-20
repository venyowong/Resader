using JWT.Algorithms;
using JWT.Builder;
using Newtonsoft.Json.Linq;
using Resader.Common.Extensions;
using Resader.Common.Entities;
using System;
using Serilog;

namespace Resader.Api.Services;

public static class JwtService
{
    private const string _secret = "e6e8d28203cc453baf87b9aadf34bb15";

    public static string GetToken(User user)
    {
        return new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(_secret)
            .AddClaim("userid", user.Id)
            .AddClaim("mail", user.Mail)
            .AddClaim("role", user.Role)
            .Encode();
    }

    public static JObject ParseToken(string token)
    {
        try
        {
            return new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(_secret)
                .MustVerifySignature()
                .Decode(token)
                .ToObj<JObject>();
        }
        catch (Exception e)
        {
            Log.Error(e, "ParseToken");
            return null;
        }
    }
}
