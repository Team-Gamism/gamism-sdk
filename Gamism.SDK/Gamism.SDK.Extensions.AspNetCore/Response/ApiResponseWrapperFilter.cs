using System.Threading.Tasks;
using Gamism.SDK.Core.Network;
using Gamism.SDK.Extensions.AspNetCore.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Gamism.SDK.Extensions.AspNetCore.Response
{
    public class ApiResponseWrapperFilter : IAsyncResultFilter
    {
        private readonly ResponseWrapperOptions _options;

        public ApiResponseWrapperFilter(ResponseWrapperOptions options)
        {
            _options = options;
        }

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (IsExcluded(context.HttpContext.Request.Path.Value))
            {
                await next();
                return;
            }

            context.Result = context.Result switch
            {
                // Already wrapped: keep the response body and align the HTTP status code.
                ObjectResult { Value: ICommonApiResponse response } =>
                    new ObjectResult(response) { StatusCode = response.Code },

                // 4xx/5xx responses should pass through without wrapping.
                ObjectResult { StatusCode: >= 400 } errorResult => errorResult,

                // Null or empty results become 204 No Content.
                ObjectResult { Value: null } or EmptyResult =>
                    new StatusCodeResult(StatusCodes.Status204NoContent),

                // Normal objects are wrapped as CommonApiResponse.Success.
                ObjectResult objectResult =>
                    new OkObjectResult(CommonApiResponse.Success("OK", objectResult.Value)),

                _ => context.Result
            };

            await next();
        }

        private bool IsExcluded(string path)
            => UrlPatternMatcher.IsMatch(path, _options.NotWrappingUrls);
    }
}
