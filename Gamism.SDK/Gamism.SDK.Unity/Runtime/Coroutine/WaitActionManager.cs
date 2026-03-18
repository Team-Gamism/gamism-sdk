using Gamism.SDK.Unity.Singleton;

namespace Gamism.SDK.Unity.Coroutine
{
    /// <summary>
    /// WaitAction의 코루틴 실행 컨텍스트를 제공하는 MonoBehaviour.
    /// 직접 사용하지 않고 WaitAction을 통해 접근한다.
    /// </summary>
    internal sealed class WaitActionManager : MonoSingleton<WaitActionManager> { }
}
