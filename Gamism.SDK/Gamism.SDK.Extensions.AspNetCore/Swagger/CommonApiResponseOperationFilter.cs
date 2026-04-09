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
            var isWrapped = returnType.IsGenericType
                && returnType.GetGenericTypeDefinition() == typeof(CommonApiResponse<>)
                && returnType.GetGenericArguments()[0] != typeof(EmptyResponse);

            if (!operation.Responses.TryGetValue("200", out var response))
                return;

            // CommonApiResponse<T>인 경우 T의 스키마만 꺼내 data 필드에 사용
            OpenApiSchema? dataSchema = null;
            if (isWrapped)
            {
                var innerType = returnType.GetGenericArguments()[0];
                dataSchema = context.SchemaGenerator.GenerateSchema(innerType, context.SchemaRepository);
            }

            foreach (var content in response.Content.Values)
                content.Schema = BuildWrappedSchema(dataSchema, hideDataField: !isWrapped);
        }

        private static OpenApiSchema BuildWrappedSchema(OpenApiSchema? dataSchema, bool hideDataField)
        {
            var properties = new Dictionary<string, OpenApiSchema>
            {
                ["status"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("OK") },
                ["code"] = new OpenApiSchema { Type = "integer", Format = "int32", Example = new OpenApiInteger(200) },
                ["message"] = new OpenApiSchema { Type = "string", Example = new OpenApiString("OK") },
            };

            if (!hideDataField && dataSchema != null)
                properties["data"] = dataSchema;

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
