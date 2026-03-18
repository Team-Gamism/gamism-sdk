using System.Net;

namespace Gamism.SDK.Extensions.AspNetCore.Exceptions
{
    public class ConflictException : SdkException
    {
        public ConflictException(string message) : base(HttpStatusCode.Conflict, message) { }
    }
}
