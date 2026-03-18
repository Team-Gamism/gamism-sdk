using UnityEngine;

namespace Gamism.SDK.Unity.Singleton
{
    /// <summary>
    /// Unity MonoBehaviour 환경에서 사용하는 싱글턴 베이스 클래스.
    /// 씬 전환 시 파괴되지 않으며, 중복 인스턴스는 즉시 제거된다.
    /// 순수 C# 환경에서는 SingletonBase&lt;T&gt;를 사용한다.
    /// </summary>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T _instance;
        private static bool _isQuitting;

        public static T Instance
        {
            get
            {
                if (_isQuitting)
                    return null;

                if (_instance == null)
                    _instance = FindObjectOfType<T>();

                return _instance;
            }
        }

        public static bool HasInstance => _instance != null && !_isQuitting;

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = (T)this;
            DontDestroyOnLoad(gameObject);

            OnAwake();
        }

        protected virtual void OnAwake() { }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        private void OnApplicationQuit()
        {
            _isQuitting = true;
        }
    }
}
