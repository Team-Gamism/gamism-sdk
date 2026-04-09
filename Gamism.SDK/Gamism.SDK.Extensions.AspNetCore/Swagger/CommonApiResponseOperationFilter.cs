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
    /// Wraps 200 response schemas with CommonApiResponse.
    /// If the action already returns CommonApiResponse, only the inner schema is used for data.
    /// </summary>
    public class CommonApiResponseOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var returnType = UnwrapType(context.MethodInfo.ReturnType);
            var isAlreadyWrapped = returnType.IsGenericType
                && returnType.GetGenericTypeDefinition() == typeof(CommonApiResponse<>);

            if (!operation.Responses.TryGetValue("200", out var response))
                return;

            foreach (var content in response.Content.Values)
            {
                var innerType = isAlreadyWrapped
                    ? returnType.GetGenericArguments()[0]
                    : returnType;
                var hideDataField = innerType == typeof(EmptyResponse)
                    || innerType == typeof(void);
                var dataSchema = hideDataField
                    ? null
                    : isAlreadyWrapped
                        ? context.SchemaGenerator.GenerateSchema(innerType, context.SchemaRepository)
                        : content.Schema;

                content.Schema = BuildWrappedSchema(dataSchema, hideDataField);
            }
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
            // Task<T> -> T
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
                type = type.GetGenericArguments()[0];

            // ActionResult<T> -> T
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ActionResult<>))
                type = type.GetGenericArguments()[0];

            return type;
        }
    }
}
