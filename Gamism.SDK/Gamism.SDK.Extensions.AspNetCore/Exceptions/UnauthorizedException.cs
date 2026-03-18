using System.Net;

namespace Gamism.SDK.Extensions.AspNetCore.Exceptions
{
    public class UnauthorizedException : SdkException
    {
        public UnauthorizedException(string message) : base(HttpStatusCode.Unauthorized, message) { }
    }
}
