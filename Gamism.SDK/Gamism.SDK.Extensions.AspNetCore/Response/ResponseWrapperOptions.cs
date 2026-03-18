namespace Gamism.SDK.Extensions.AspNetCore.Response
{
    public class ResponseWrapperOptions
    {
        /// <summary>
        /// 응답 래핑 기능 활성화 여부.
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// 래핑에서 제외할 URL 패턴 목록. ** 와일드카드를 지원한다.
        /// 예: "/health", "/swagger/**"
        /// </summary>
        public string[] NotWrappingUrls { get; set; } = [];
    }
}
