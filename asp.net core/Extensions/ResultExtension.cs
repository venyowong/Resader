using Resader.Models.Response;

namespace Resader.Extensions
{
    public static class ResultExtension
    {
        public static Result<T> ToResult<T>(this T data, string message = null)
        {
            return new Result<T>
            {
                Code = 0,
                Data = data,
                Message = message
            };
        }
    }
}