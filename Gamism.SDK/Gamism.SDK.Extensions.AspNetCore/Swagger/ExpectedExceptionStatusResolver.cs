using System;
using System.Collections.Concurrent;
using System.Net;
using Gamism.SDK.Extensions.AspNetCore.Exceptions;

namespace Gamism.SDK.Extensions.AspNetCore.Swagger
{
    /// <summary>
    /// 예외 타입에서 <see cref="ExpectedException"/> 상속 계층을 따라
    /// 고정 상태 코드를 해석한다. 선언 타입부터 상위로 올라가며
    /// string 한 개를 받거나 매개변수가 없는 생성자로 인스턴스화 가능한
    /// 첫 타입의 StatusCode를 사용한다. 결과는 타입별로 캐싱한다.
    /// </summary>
    internal static class ExpectedExceptionStatusResolver
    {
        private static readonly ConcurrentDictionary<Type, HttpStatusCode?> Cache =
            new ConcurrentDictionary<Type, HttpStatusCode?>();

        public static HttpStatusCode? Resolve(Type exceptionType)
        {
            if (exceptionType == null)
                return null;

            return Cache.GetOrAdd(exceptionType, ResolveUncached);
        }

        private static HttpStatusCode? ResolveUncached(Type exceptionType)
        {
            for (var type = exceptionType; type != null && type != typeof(Exception); type = type.BaseType)
            {
                if (!typeof(ExpectedException).IsAssignableFrom(type))
                    continue;

                var ctor = type.GetConstructor(new[] { typeof(string) })
                    ?? type.GetConstructor(Type.EmptyTypes);
                if (ctor == null)
                    continue;

                try
                {
                    var args = ctor.GetParameters().Length == 0
                        ? null
                        : new object[] { string.Empty };
                    var instance = (ExpectedException)ctor.Invoke(args);
                    return instance.StatusCode;
                }
                catch
                {
                    // 생성자 실행이 실패하면 상위 타입에서 다시 시도한다.
                }
            }

            return null;
        }
    }
}
