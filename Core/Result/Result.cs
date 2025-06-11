namespace Core.Result
{
    public class Result<T> where T : class
    {
        public bool IsSuccess { get; }
        public string? ErrorMessage { get; }
        public T? Data { get; }
        private Result(bool issuccess, T? data, string? er_mess)
        {
            this.IsSuccess = issuccess;
            this.ErrorMessage = er_mess;
            this.Data = data;
        }

        public static Result<T> Success(T? data) => new Result<T>(true, data, null);
        public static Result<T> Fail(T? data, string? er_mess) => new Result<T>(false, data, er_mess);
    }
}
