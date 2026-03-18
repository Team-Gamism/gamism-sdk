using System.Net;

namespace Gamism.SDK.Core.Exceptions
{
    public class NotFoundException : SdkException
    {
        public NotFoundException(string message) : base(HttpStatusCode.NotFound, message) { }
    }
}
