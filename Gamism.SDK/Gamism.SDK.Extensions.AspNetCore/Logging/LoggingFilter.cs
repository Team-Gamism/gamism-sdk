using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Gamism.SDK.Extensions.AspNetCore.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Gamism.SDK.Extensions.AspNetCore.Logging
{
    public class LoggingFilter : IMiddleware
    {
        private readonly ILogger<LoggingFilter> _logger;
        private readonly LoggingOptions _options;

        public LoggingFilter(ILogger<LoggingFilter> logger, LoggingOptions options)
        {
            _logger = logger;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (IsExcluded(context.Request.Path.Value))
            {
                await next(context);
                return;
            }

            _logger.LogInformation("[Request] {Method} {Path}{QueryString}",
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString);

            var sw = Stopwatch.StartNew();
            await next(context);
            sw.Stop();

            _logger.LogInformation("[Response] {StatusCode} ({Elapsed}ms)",
                context.Response.StatusCode,
                sw.ElapsedMilliseconds);
        }

        private bool IsExcluded(string path)
        {
            if (string.IsNullOrEmpty(path) || _options.NotLoggingUrls.Length == 0)
                return false;

            foreach (var pattern in _options.NotLoggingUrls)
            {
                var regex = "^" + Regex.Escape(pattern)
                    .Replace("\\*\\*", ".*")
                    .Replace("\\*", "[^/]*") + "$";

                if (Regex.IsMatch(path, regex, RegexOptions.IgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
