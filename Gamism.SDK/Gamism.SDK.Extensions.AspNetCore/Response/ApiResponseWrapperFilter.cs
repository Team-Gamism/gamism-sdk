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
                // 이미 CommonApiResponse인 경우 → HTTP 상태코드만 맞추고 통과
                ObjectResult { Value: ICommonApiResponse response } =>
                    new ObjectResult(response) { StatusCode = response.Code },

                // null 또는 빈 응답 → 204 No Content
                ObjectResult { Value: null } or EmptyResult =>
                    new StatusCodeResult(StatusCodes.Status204NoContent),

                // 일반 객체 → CommonApiResponse.Success로 래핑
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
