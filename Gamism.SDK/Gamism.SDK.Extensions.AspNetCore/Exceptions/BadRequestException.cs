using System.Net;

namespace Gamism.SDK.Extensions.AspNetCore.Exceptions
{
    public class BadRequestException : ExpectedException
    {
        public BadRequestException(string message) : base(HttpStatusCode.BadRequest, message) { }
    }
}
