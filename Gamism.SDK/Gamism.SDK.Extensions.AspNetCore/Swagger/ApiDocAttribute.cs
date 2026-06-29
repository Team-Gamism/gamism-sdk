using System;

namespace Gamism.SDK.Extensions.AspNetCore.Swagger
{
    /// <summary>
    /// Swagger 문서에 표시할 API 요약/상세 설명을 지정한다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class ApiDocAttribute : Attribute
    {
        public string Summary { get; }
        public string Description { get; }

        /// <param name="summary">엔드포인트 한 줄 요약.</param>
        /// <param name="description">선택적인 상세 설명.</param>
        public ApiDocAttribute(string summary, string description = null)
        {
            Summary = summary;
            Description = description;
        }
    }
}
