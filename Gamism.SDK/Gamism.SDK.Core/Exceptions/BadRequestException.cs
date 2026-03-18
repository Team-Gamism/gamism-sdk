using System.Net;

namespace Gamism.SDK.Core.Exceptions
{
    public class BadRequestException : SdkException
    {
        public BadRequestException(string message) : base(HttpStatusCode.BadRequest, message) { }
    }
}
