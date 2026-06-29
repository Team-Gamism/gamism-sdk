using System;
using System.Net;

namespace Gamism.SDK.Extensions.AspNetCore.Swagger
{
    /// <summary>
    /// 엔드포인트가 반환할 수 있는 실패 응답을 Swagger 문서에 추가한다.
    /// 예외 타입을 지정하면 <see cref="Exceptions.ExpectedException"/> 상속 계층에서
    /// 상태 코드를 자동으로 해석한다. 예외와 무관한 응답은 상태 코드를 직접 지정할 수 있다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ApiErrorAttribute : Attribute
    {
        public Type ExceptionType { get; }
        public HttpStatusCode? StatusCode { get; }
        public string Message { get; }

        /// <param name="exceptionType">ExpectedException을 상속한 예외 타입.</param>
        /// <param name="message">선택적인 예시 메시지. 생략하면 상태 이름을 사용한다.</param>
        public ApiErrorAttribute(Type exceptionType, string message = null)
        {
            ExceptionType = exceptionType;
            Message = message;
        }

        /// <param name="statusCode">응답 상태 코드.</param>
        /// <param name="message">선택적인 예시 메시지. 생략하면 상태 이름을 사용한다.</param>
        public ApiErrorAttribute(HttpStatusCode statusCode, string message = null)
        {
            StatusCode = statusCode;
            Message = message;
        }
    }
}
