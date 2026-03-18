using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamism.SDK.Unity.Coroutine
{
    /// <summary>
    /// 콜백 기반 지연 실행 유틸리티.
    /// </summary>
    public static class WaitAction
    {
        private static readonly Dictionary<int, WaitForSeconds> _cache = new Dictionary<int, WaitForSeconds>();

        private static int ToKey(float seconds) => Mathf.RoundToInt(seconds * 1000f);

        /// <summary>
        /// 지정한 시간 후 콜백을 실행한다.
        /// </summary>
        public static UnityEngine.Coroutine Wait(float seconds, Action callback)
        {
            return WaitActionManager.Instance.StartCoroutine(Internal_Wait(seconds, callback));
        }

        /// <summary>
        /// 조건이 충족될 때 콜백을 실행한다.
        /// </summary>
        /// <param name="timeOut">타임아웃(초). 음수면 무한 대기.</param>
        public static UnityEngine.Coroutine WaitUntil(Func<bool> condition, Action callback, float timeOut = -1)
        {
            return WaitActionManager.Instance.StartCoroutine(Internal_WaitUntil(condition, callback, timeOut));
        }

        private static IEnumerator Internal_Wait(float seconds, Action callback)
        {
            var key = ToKey(seconds);
            if (!_cache.TryGetValue(key, out var wait))
            {
                wait = new WaitForSeconds(seconds);
                _cache[key] = wait;
            }

            yield return wait;
            callback?.Invoke();
        }

        private static IEnumerator Internal_WaitUntil(Func<bool> condition, Action callback, float timeOut)
        {
            float timer = 0f;
            bool isSuccess = false;

            yield return new WaitUntil(() =>
            {
                timer += Time.deltaTime;

                if (condition())
                {
                    isSuccess = true;
                    return true;
                }

                return timeOut >= 0f && timer >= timeOut;
            });

            if (isSuccess)
                callback?.Invoke();
        }
    }
}
