using Microsoft.AspNetCore.Components;
using System.Collections.Specialized;
using System.Web;

namespace Resader.Wasm.Extensions
{
    public static class NavigationManagerExtension
    {
        // get entire querystring name/value collection
        public static NameValueCollection GetQueryString(this NavigationManager navigationManager)
        {
            return HttpUtility.ParseQueryString(new Uri(navigationManager.Uri).Query);
        }

        // get single querystring value with specified key
        public static string GetQueryParameter(this NavigationManager navigationManager, string key)
        {
            return navigationManager.GetQueryString()[key] ?? string.Empty;
        }
    }
}
