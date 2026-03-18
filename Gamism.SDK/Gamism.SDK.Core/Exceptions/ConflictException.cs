using System.Net;

namespace Gamism.SDK.Core.Exceptions
{
    public class ConflictException : SdkException
    {
        public ConflictException(string message) : base(HttpStatusCode.Conflict, message) { }
    }
}
