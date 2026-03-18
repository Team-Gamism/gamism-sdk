using Gamism.SDK.Extensions.AspNetCore.Options;
using Gamism.SDK.Extensions.AspNetCore.Response;
using Gamism.SDK.Extensions.AspNetCore.Swagger;

namespace Gamism.SDK.Extensions.AspNetCore
{
    public class GamismSdkOptions
    {
        public ExceptionOptions Exception { get; set; } = new();
        public ResponseWrapperOptions Response { get; set; } = new();
        public SwaggerOptions Swagger { get; set; } = new();
        public LoggingOptions Logging { get; set; } = new();
    }
}
