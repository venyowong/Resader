using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Resader.Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resader.Api.Attributes
{
    public class JwtValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("token", out var value) || !value.Any())
            {
                context.Result = new StatusCodeResult(401);
                return;
            }
            var token = value.First();
            var obj = JwtService.ParseToken(token);
            if (obj == null)
            {
                context.Result = new StatusCodeResult(401);
                return;
            }

            context.HttpContext.Items.Add("token", obj);

            base.OnActionExecuting(context);
        }
    }
}
