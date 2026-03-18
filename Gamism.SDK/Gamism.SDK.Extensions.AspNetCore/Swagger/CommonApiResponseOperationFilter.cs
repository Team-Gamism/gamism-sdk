using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Gamism.SDK.Core.Network;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Gamism.SDK.Extensions.AspNetCore.Swagger
{
    /// <summary>
    /// 모든 200 응답 스키마를 CommonApiResponse 형태로 래핑하는 Swagger 필터.
    /// 반환 타입이 이미 ICommonApiResponse이면 data 필드를 숨긴다.
    /// </summary>
    public class CommonApiResponseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var returnType = UnwrapType(context.MethodInfo.ReturnType);
            var hideDataField = typeof(ICommonApiResponse).IsAssignableFrom(returnType);

            if (!operation.Responses.TryGetValue("200", out var response))
                return;

            foreach (var content in response.Content.Values)
            {
                var originalSchema = content.Schema;
                content.Schema = BuildWrappedSchema(originalSchema, hideDataField);
            }
        }

        private static OpenApiSchema BuildWrappedSchema(OpenApiSchema originalSchema, bool hideDataField)
        {
            var properties = new Dictionary<string, OpenApiSchema>
            {
                ["status"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("OK") },
                ["code"] = new OpenApiSchema { Type = "integer", Format = "int32", Example = new OpenApiInteger(200) },
                ["message"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("OK") },
            };

            if (!hideDataField && originalSchema != null)
                properties["data"] = originalSchema;

            return new OpenApiSchema { Type = "object", Properties = properties };
        }

        private static Type UnwrapType(Type type)
        {
            // Task<T> → T
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
                type = type.GetGenericArguments()[0];

            // ActionResult<T> → T
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ActionResult<>))
                type = type.GetGenericArguments()[0];

            return type;
        }
    }
}
