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
    }
}
