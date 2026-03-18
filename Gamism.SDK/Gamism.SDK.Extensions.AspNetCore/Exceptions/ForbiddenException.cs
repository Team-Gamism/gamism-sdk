using System.Net;

namespace Gamism.SDK.Extensions.AspNetCore.Exceptions
{
    public class ForbiddenException : ExpectedException
    {
        public ForbiddenException(string message) : base(HttpStatusCode.Forbidden, message) { }
    }
}
