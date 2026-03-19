using System;
using Gamism.SDK.Extensions.AspNetCore.Options;
using Gamism.SDK.Extensions.AspNetCore.Response;
using Gamism.SDK.Extensions.AspNetCore.Swagger;

namespace Gamism.SDK.Extensions.AspNetCore
{
    public class GamismSdkOptions
    {
        public ExceptionOptions Exception { get; } = new();
        public ResponseWrapperOptions Response { get; } = new();
        public SwaggerOptions Swagger { get; } = new();
        public LoggingOptions Logging { get; } = new();

        public GamismSdkOptions ConfigureException(Action<ExceptionOptions> configure)
        {
            configure(Exception);
            return this;
        }

        public GamismSdkOptions ConfigureResponse(Action<ResponseWrapperOptions> configure)
        {
            configure(Response);
            return this;
        }

        public GamismSdkOptions ConfigureSwagger(Action<SwaggerOptions> configure)
        {
            configure(Swagger);
            return this;
        }

        public GamismSdkOptions ConfigureLogging(Action<LoggingOptions> configure)
        {
            configure(Logging);
            return this;
        }
    }
}
