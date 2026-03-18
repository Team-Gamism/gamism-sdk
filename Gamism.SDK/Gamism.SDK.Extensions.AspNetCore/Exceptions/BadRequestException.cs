using System.Net;

namespace Gamism.SDK.Extensions.AspNetCore.Exceptions
{
    public class BadRequestException : SdkException
    {
        public BadRequestException(string message) : base(HttpStatusCode.BadRequest, message) { }
    }
}
