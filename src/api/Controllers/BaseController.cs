using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Resader.Api.Controllers
{
    public class BaseController : Controller
    {
        protected string GetUserId()
        {
            if (!this.HttpContext.Items.TryGetValue("token", out var obj))
            {
                return string.Empty;
            }
            var jObj = obj as JObject;
            if (jObj == null)
            {
                return string.Empty;
            }

            return jObj["userid"].ToString();
        }

        protected string GetMail()
        {
            if (!this.HttpContext.Items.TryGetValue("token", out var obj))
            {
                return string.Empty;
            }
            var jObj = obj as JObject;
            if (jObj == null)
            {
                return string.Empty;
            }

            return jObj["mail"].ToString();
        }
    }
}
