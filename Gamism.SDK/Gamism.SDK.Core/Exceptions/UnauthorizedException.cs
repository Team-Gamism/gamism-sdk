using System.Net;

namespace Gamism.SDK.Core.Exceptions
{
    public class UnauthorizedException : SdkException
    {
        public UnauthorizedException(string message) : base(HttpStatusCode.Unauthorized, message) { }
    }
}
