using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace Resader.Api.Extensions;
public static class HttpRequestExtension
{
    public static async Task<string> ReadBody(this HttpRequest request)
    {
        if (request == null)
        {
            return string.Empty;
        }

        using (var reader = new StreamReader(request.Body))
        {
            return await reader.ReadToEndAsync();
        }
    }
}
