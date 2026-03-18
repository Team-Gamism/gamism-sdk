using System.Net;

namespace Gamism.SDK.Extensions.AspNetCore.Exceptions
{
    public class ForbiddenException : SdkException
    {
        public ForbiddenException(string message) : base(HttpStatusCode.Forbidden, message) { }
    }
}
