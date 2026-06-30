using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Gamism.SDK.Extensions.AspNetCore.Logging
{
    /// <summary>
    /// <see cref="LogTimeZone"/>를 IANA 타임존 ID로 매핑하고
    /// 해당 시간대의 현재 시각을 반환한다.
    /// </summary>
    internal static class LogTimeZoneResolver
    {
        private static readonly Dictionary<LogTimeZone, string> IanaIds = new()
        {
            [LogTimeZone.Utc] = "UTC",
            [LogTimeZone.Korea] = "Asia/Seoul",
            [LogTimeZone.Japan] = "Asia/Tokyo",
            [LogTimeZone.China] = "Asia/Shanghai",
            [LogTimeZone.India] = "Asia/Kolkata",
            [LogTimeZone.UsEastern] = "America/New_York",
            [LogTimeZone.UsPacific] = "America/Los_Angeles",
            [LogTimeZone.UnitedKingdom] = "Europe/London",
            [LogTimeZone.Germany] = "Europe/Berlin"
        };

        private static readonly ConcurrentDictionary<LogTimeZone, TimeZoneInfo> TimeZoneCache = new();

        /// <summary>지정한 시간대의 현재 시각을 반환한다. 미정의 값은 UTC로 처리한다.</summary>
        public static DateTime Now(LogTimeZone timeZone)
        {
            if (timeZone == LogTimeZone.Utc || !IanaIds.TryGetValue(timeZone, out var ianaId))
                return DateTime.UtcNow;

            var info = TimeZoneCache.GetOrAdd(timeZone, _ => TimeZoneInfo.FindSystemTimeZoneById(ianaId));
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, info);
        }
    }
}
