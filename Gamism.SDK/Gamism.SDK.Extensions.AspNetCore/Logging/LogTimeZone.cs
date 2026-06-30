namespace Gamism.SDK.Extensions.AspNetCore.Logging
{
    /// <summary>
    /// 로그 타임스탬프에 사용할 국가별 시간대.
    /// 각 항목은 IANA 타임존 ID로 매핑되어 Linux/Windows 모두에서 동작한다.
    /// </summary>
    public enum LogTimeZone
    {
        /// <summary>UTC (협정 세계시).</summary>
        Utc,

        /// <summary>대한민국 — Asia/Seoul.</summary>
        Korea,

        /// <summary>일본 — Asia/Tokyo.</summary>
        Japan,

        /// <summary>중국 — Asia/Shanghai.</summary>
        China,

        /// <summary>인도 — Asia/Kolkata.</summary>
        India,

        /// <summary>미국 동부 — America/New_York.</summary>
        UsEastern,

        /// <summary>미국 서부 — America/Los_Angeles.</summary>
        UsPacific,

        /// <summary>영국 — Europe/London.</summary>
        UnitedKingdom,

        /// <summary>독일(중부 유럽) — Europe/Berlin.</summary>
        Germany
    }
}
