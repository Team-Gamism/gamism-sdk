namespace Gamism.SDK.Core.Result
{
    /// <summary>
    /// 예외 없이 성공/실패를 표현하는 타입.
    /// 반환값이 없는 작업에 사용한다.
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; }
        public string Error { get; }

        protected Result(bool isSuccess, string error)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Ok() => new Result(true, null);
        public static Result Fail(string error) => new Result(false, error);

        public static Result<T> Ok<T>(T value) => Result<T>.Ok(value);
        public static Result<T> Fail<T>(string error) => Result<T>.Fail(error);
    }

    /// <summary>
    /// 예외 없이 성공/실패를 표현하는 타입.
    /// 반환값이 있는 작업에 사용한다.
    /// </summary>
    public class Result<T> : Result
    {
        public T Value { get; }

        private Result(bool isSuccess, T value, string error) : base(isSuccess, error)
        {
            Value = value;
        }

        public static Result<T> Ok(T value) => new Result<T>(true, value, null);
        public new static Result<T> Fail(string error) => new Result<T>(false, default, error);
    }
}
