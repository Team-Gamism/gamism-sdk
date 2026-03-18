using System.Net;

namespace Gamism.SDK.Extensions.AspNetCore.Exceptions
{
    public class UnauthorizedException : ExpectedException
    {
        public UnauthorizedException(string message) : base(HttpStatusCode.Unauthorized, message) { }
    }
}
