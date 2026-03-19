using System;
using Gamism.SDK.Extensions.AspNetCore.Exceptions;
using Gamism.SDK.Extensions.AspNetCore.Logging;
using Gamism.SDK.Extensions.AspNetCore.Response;
using Gamism.SDK.Extensions.AspNetCore.Swagger;
using Gamism.SDK.Extensions.AspNetCore.Utils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Gamism.SDK.Extensions.AspNetCore
{
    public static class GamismSdkExtensions
    {
        /// <summary>
        /// Gamism SDK 서버 기능을 등록한다.
        /// 각 기능은 기본적으로 활성화되어 있으며 옵션으로 제어할 수 있다.
        /// </summary>
        public static IServiceCollection AddGamismSdk(
            this IServiceCollection services,
            Action<GamismSdkOptions> configure = null)
        {
            var options = new GamismSdkOptions();
            configure?.Invoke(options);

            services.AddSingleton(options);

            if (options.Exception.Enabled)
            {
                services.AddExceptionHandler<GlobalExceptionHandler>();
                services.AddProblemDetails();
            }

            if (options.Response.Enabled)
            {
                services.AddSingleton(options.Response);
                services.AddScoped<ApiResponseWrapperFilter>();
                services.Configure<MvcOptions>(mvc => mvc.Filters.AddService<ApiResponseWrapperFilter>());
            }

            if (options.Swagger.Enabled)
            {
                services.AddSingleton(options.Swagger);
                services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc(options.Swagger.Version, new OpenApiInfo
                    {
                        Title = options.Swagger.Title,
                        Description = options.Swagger.Description,
                        Version = options.Swagger.Version,
                    });

                    c.OperationFilter<CommonApiResponseOperationFilter>();

                    c.DocInclusionPredicate((_, apiDesc) =>
                        UrlPatternMatcher.IsMatch("/" + apiDesc.RelativePath, options.Swagger.PathsToMatch));
                });
            }

            if (options.Logging.Enabled)
            {
                services.AddSingleton(options.Logging);
                services.AddTransient<LoggingFilter>();
            }

            return services;
        }

        /// <summary>
        /// Gamism SDK 미들웨어를 등록한다.
        /// AddGamismSdk() 이후 호출해야 한다.
        /// </summary>
        public static IApplicationBuilder UseGamismSdk(this IApplicationBuilder app)
        {
            var options = app.ApplicationServices.GetRequiredService<GamismSdkOptions>();

            if (options.Exception.Enabled)
                app.UseExceptionHandler();

            if (options.Logging.Enabled)
                app.UseMiddleware<LoggingFilter>();

            if (options.Swagger.Enabled)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                    c.SwaggerEndpoint(
                        $"/swagger/{options.Swagger.Version}/swagger.json",
                        options.Swagger.Title));
            }

            return app;
        }
    }
}
