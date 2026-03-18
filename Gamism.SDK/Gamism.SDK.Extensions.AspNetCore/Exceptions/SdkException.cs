using System;
using System.Net;

namespace Gamism.SDK.Extensions.AspNetCore.Exceptions
{
    /// <summary>
    /// HTTP 상태코드를 포함하는 SDK 예외 베이스 클래스.
    /// </summary>
    public class SdkException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        public SdkException(HttpStatusCode statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
