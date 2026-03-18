using System;

namespace Gamism.SDK.Core.Singleton
{
    /// <summary>
    /// 순수 C# 환경(AspNetCore, 테스트 등)에서 사용하는 스레드 안전 싱글턴.
    /// Unity에서는 MonoSingleton&lt;T&gt;를 사용한다.
    /// </summary>
    public abstract class SingletonBase<T> where T : SingletonBase<T>
    {
        private static T _instance;
        private static readonly object _lock = new object();

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = (T)Activator.CreateInstance(typeof(T), nonPublic: true);
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// 테스트 격리 또는 명시적 재초기화 목적으로만 사용.
        /// </summary>
        protected static void ClearInstance()
        {
            lock (_lock)
            {
                _instance = null;
            }
        }

        protected SingletonBase() { }
    }
}
