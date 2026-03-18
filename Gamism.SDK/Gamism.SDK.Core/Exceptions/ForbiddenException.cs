using System.Net;

namespace Gamism.SDK.Core.Exceptions
{
    public class ForbiddenException : SdkException
    {
        public ForbiddenException(string message) : base(HttpStatusCode.Forbidden, message) { }
    }
}
