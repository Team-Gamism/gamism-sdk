using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Gamism.SDK.Extensions.AspNetCore.Exceptions;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Gamism.SDK.Extensions.AspNetCore.Swagger
{
    /// <summary>
    /// [ApiDoc] / [ApiError] attribute를 읽어 Swagger 문서에
    /// 요약·상세 설명과 실패 응답(CommonApiResponse 형식)을 추가한다.
    /// </summary>
    public class ApiDocumentationOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var method = context.MethodInfo;
            if (method == null)
                return;

            ApplyDoc(operation, method);
            ApplyErrors(operation, method);
        }

        private static void ApplyDoc(OpenApiOperation operation, MethodInfo method)
        {
            var doc = method.GetCustomAttribute<ApiDocAttribute>();
            if (doc == null)
                return;

            operation.Summary = doc.Summary;
            if (!string.IsNullOrEmpty(doc.Description))
                operation.Description = doc.Description;
        }

        private static void ApplyErrors(OpenApiOperation operation, MethodInfo method)
        {
            var errors = method.GetCustomAttributes<ApiErrorAttribute>()
                .Concat(method.DeclaringType?.GetCustomAttributes<ApiErrorAttribute>()
                    ?? Enumerable.Empty<ApiErrorAttribute>());

            foreach (var error in errors)
            {
                var status = error.StatusCode
                    ?? ExpectedExceptionStatusResolver.Resolve(error.ExceptionType);
                if (status == null)
                    continue;

                AddErrorResponse(operation, status.Value, error.Message);
            }
        }

        private static void AddErrorResponse(OpenApiOperation operation, HttpStatusCode status, string message)
        {
            var code = ((int)status).ToString();
            var displayMessage = string.IsNullOrEmpty(message) ? status.ToString() : message;

            // 동일 상태 코드가 이미 있으면 설명만 누적한다. (OpenAPI는 상태 코드당 응답 1개)
            if (operation.Responses.TryGetValue(code, out var existing))
            {
                if (!string.IsNullOrEmpty(message)
                    && existing.Description != null
                    && !existing.Description.Contains(displayMessage))
                {
                    existing.Description += " / " + displayMessage;
                }
                return;
            }

            var statusText = status.ToString().ToUpper();

            operation.Responses[code] = new OpenApiResponse
            {
                Description = displayMessage,
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["status"] = new OpenApiSchema { Type = "string" },
                                ["code"] = new OpenApiSchema { Type = "integer", Format = "int32" },
                                ["message"] = new OpenApiSchema { Type = "string" },
                            },
                        },
                        Example = new OpenApiObject
                        {
                            ["status"] = new OpenApiString(statusText),
                            ["code"] = new OpenApiInteger((int)status),
                            ["message"] = new OpenApiString(displayMessage),
                        },
                    },
                },
            };
        }
    }
}
