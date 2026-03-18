using System.Net;

namespace Gamism.SDK.Extensions.AspNetCore.Exceptions
{
    public class NotFoundException : ExpectedException
    {
        public NotFoundException(string message) : base(HttpStatusCode.NotFound, message) { }
    }
}
