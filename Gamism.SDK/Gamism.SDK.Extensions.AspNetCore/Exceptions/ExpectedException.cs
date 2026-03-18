using System;
using System.Net;

namespace Gamism.SDK.Extensions.AspNetCore.Exceptions
{
    /// <summary>
    /// 예상된 예외. 버그가 아닌 비즈니스 로직 상 발생 가능한 오류를 표현한다.
    /// GlobalExceptionHandler에서 스택 트레이스를 로그에 남기지 않는다.
    /// </summary>
    public class ExpectedException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public ExpectedException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }

        public ExpectedException(HttpStatusCode statusCode) : base(statusCode.ToString())
        {
            StatusCode = statusCode;
        }
    }
}
