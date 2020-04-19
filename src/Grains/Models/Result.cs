namespace Resader.Grains.Models
{
    public class Result
    {
        /// <summary>
        /// 0 正常 1 参数异常 2 系统异常
        /// </summary>
        public int Code { get; set; }

        public string Message { get; set; }
    }

    public class Result<T> : Result
    {
        public T Data { get; set; }
    }
}
