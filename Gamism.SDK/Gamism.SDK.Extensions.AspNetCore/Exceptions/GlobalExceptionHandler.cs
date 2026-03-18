using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Gamism.SDK.Core.Exceptions;
using Gamism.SDK.Core.Network;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Gamism.SDK.Extensions.AspNetCore.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
        {
            var (statusCode, message) = exception switch
            {
                SdkException ex => (ex.StatusCode, ex.Message),
                _ => (HttpStatusCode.InternalServerError, "서버 내부 오류가 발생했습니다.")
            };

            if (statusCode == HttpStatusCode.InternalServerError)
                _logger.LogError(exception, "Unhandled exception occurred");

            context.Response.StatusCode = (int)statusCode;
            await context.Response.WriteAsJsonAsync(
                CommonApiResponse.Error(message, statusCode), ct);

            return true;
        }
    }
}
