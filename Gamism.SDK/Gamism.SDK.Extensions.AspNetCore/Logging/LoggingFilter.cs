using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using Gamism.SDK.Extensions.AspNetCore.Options;
using Gamism.SDK.Extensions.AspNetCore.Utils;
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

            _logger.LogInformation("[{Timestamp}] [Request] {Method} {Path}{QueryString}",
                Timestamp(),
                context.Request.Method,
                context.Request.Path,
                context.Request.QueryString);

            var sw = Stopwatch.StartNew();
            await next(context);
            sw.Stop();

            _logger.LogInformation("[{Timestamp}] [Response] {StatusCode} ({Elapsed}ms)",
                Timestamp(),
                context.Response.StatusCode,
                sw.ElapsedMilliseconds);
        }

        private string Timestamp()
            => LogTimeZoneResolver.Now(_options.TimeZone).ToString("yyyy/MM/dd HH:mm", CultureInfo.InvariantCulture);

        private bool IsExcluded(string path)
            => UrlPatternMatcher.IsMatch(path, _options.NotLoggingUrls);
    }
}
