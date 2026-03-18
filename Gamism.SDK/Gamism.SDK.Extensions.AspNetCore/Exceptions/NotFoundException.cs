using System.Net;

namespace Gamism.SDK.Extensions.AspNetCore.Exceptions
{
    public class NotFoundException : SdkException
    {
        public NotFoundException(string message) : base(HttpStatusCode.NotFound, message) { }
    }
}
