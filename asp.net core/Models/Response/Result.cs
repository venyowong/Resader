namespace Resader.Models.Response
{
    public class Result
    {
        /// <summary>
        /// -1 失败 0 正常 1 参数异常 2 系统异常
        /// </summary>
        public int Code { get; set; }

        public string Message { get; set; }

        public static Result CreateSuccessResult(string message = null)
        {
            return new Result
            {
                Code = 0,
                Message = message
            };
        }

        public static Result CreateFailureResult(int code = -1, string message = null)
        {
            return new Result
            {
                Code = code,
                Message = message
            };
        }
    }

    public class Result<T> : Result
    {
        public T Data { get; set; }

        public static Result<T> CreateFailureResult(int code = -1, string message = null)
        {
            return new Result<T>
            {
                Code = code,
                Message = message
            };
        }
    }
}