using System.Text.RegularExpressions;

namespace Gamism.SDK.Extensions.AspNetCore.Utils
{
    internal static class UrlPatternMatcher
    {
        /// <summary>
        /// 주어진 URL이 패턴 목록 중 하나와 일치하는지 확인한다.
        /// `**`는 경로 구분자를 포함한 모든 문자, `*`는 경로 구분자를 제외한 모든 문자와 매칭된다.
        /// </summary>
        public static bool IsMatch(string url, string[] patterns)
        {
            if (string.IsNullOrEmpty(url) || patterns == null || patterns.Length == 0)
                return false;

            foreach (var pattern in patterns)
            {
                var regex = "^" + Regex.Escape(pattern)
                    .Replace("\\*\\*", ".*")
                    .Replace("\\*", "[^/]*") + "$";

                if (Regex.IsMatch(url, regex, RegexOptions.IgnoreCase))
                    return true;
            }

            return false;
        }
    }
}
