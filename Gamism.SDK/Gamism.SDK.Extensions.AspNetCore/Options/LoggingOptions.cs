using Gamism.SDK.Extensions.AspNetCore.Logging;

namespace Gamism.SDK.Extensions.AspNetCore.Options
{
    public class LoggingOptions
    {
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 로깅에서 제외할 URL 패턴 목록. ** 와일드카드를 지원한다.
        /// 예: "/health", "/swagger/**"
        /// </summary>
        public string[] NotLoggingUrls { get; set; } = [];

        /// <summary>
        /// 로그 타임스탬프에 사용할 국가별 시간대. 기본값은 UTC.
        /// 예: "[2026/06/30 18:15] [Request] GET /rooms"
        /// </summary>
        public LogTimeZone TimeZone { get; set; } = LogTimeZone.Utc;
    }
}
