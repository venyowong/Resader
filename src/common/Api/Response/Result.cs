using System;
using System.Collections.Generic;
using System.Text;

namespace Resader.Common.Api.Response
{
    public class Result
    {
        /// <summary>
        /// -1 失败 0 正常 1 参数异常 2 系统异常
        /// </summary>
        public int Code { get; set; }

        public string Message { get; set; }

        public static Result Success(string message = null)
        {
            return new Result
            {
                Code = 0,
                Message = message
            };
        }

        public static Result<T> Success<T>(T data) => new Result<T>
        {
            Code = 0,
            Data = data
        };

        public static Result Fail(int code = -1, string message = null)
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

        public static new Result<T> Fail(int code = -1, string message = null)
        {
            return new Result<T>
            {
                Code = code,
                Message = message
            };
        }
    }
}
