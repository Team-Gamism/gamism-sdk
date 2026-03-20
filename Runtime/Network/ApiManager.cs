using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Gamism.SDK.Core.Network;
using Gamism.SDK.Core.Network.Json;
using Gamism.SDK.Unity.Network.Json;
using Gamism.SDK.Unity.Singleton;
using UnityEngine;
using UnityEngine.Networking;

namespace Gamism.SDK.Unity.Network
{
    /// <summary>
    /// Gamism 서버와 통신하는 HTTP 클라이언트 싱글톤.
    /// 모든 응답은 <see cref="CommonApiResponse{T}"/>로 반환되며,
    /// 네트워크 오류도 예외 대신 <see cref="CommonApiResponse.Error{T}"/>로 반환됩니다.
    /// </summary>
    public class ApiManager : MonoSingleton<ApiManager>
    {
        /// <summary>모든 요청에 붙는 기본 URL (예: "https://api.example.com").</summary>
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>요청 타임아웃 (초). 기본값 30초.</summary>
        public float Timeout { get; set; } = 30f;

        /// <summary>JSON 직렬화기. 기본값은 <see cref="NewtonsoftJsonSerializer"/>.</summary>
        public IJsonSerializer Serializer { get; set; } = new NewtonsoftJsonSerializer();

        private readonly Dictionary<string, string> _defaultHeaders = new Dictionary<string, string>();

        /// <summary>모든 요청에 공통으로 포함할 헤더를 설정합니다 (예: Authorization).</summary>
        public void SetDefaultHeader(string key, string value) => _defaultHeaders[key] = value;

        /// <summary>설정된 기본 헤더를 제거합니다.</summary>
        public void RemoveDefaultHeader(string key) => _defaultHeaders.Remove(key);

        /// <summary>GET 요청을 보냅니다.</summary>
        /// <param name="path">BaseUrl 이후의 경로 (예: "/users/1").</param>
        /// <param name="callback">응답 콜백. 실패 시에도 반드시 호출됩니다.</param>
        public Coroutine Get<T>(string path, Action<CommonApiResponse<T>> callback)
        {
            var request = UnityWebRequest.Get(BaseUrl + path);
            return StartCoroutine(Send(request, callback));
        }

        /// <summary>POST 요청을 보냅니다. body는 JSON으로 직렬화됩니다.</summary>
        public Coroutine Post<T>(string path, object body, Action<CommonApiResponse<T>> callback)
        {
            var request = BuildBodyRequest("POST", path, body);
            return StartCoroutine(Send(request, callback));
        }

        /// <summary>PUT 요청을 보냅니다. body는 JSON으로 직렬화됩니다.</summary>
        public Coroutine Put<T>(string path, object body, Action<CommonApiResponse<T>> callback)
        {
            var request = BuildBodyRequest("PUT", path, body);
            return StartCoroutine(Send(request, callback));
        }

        /// <summary>DELETE 요청을 보냅니다.</summary>
        public Coroutine Delete<T>(string path, Action<CommonApiResponse<T>> callback)
        {
            var request = UnityWebRequest.Delete(BaseUrl + path);
            request.downloadHandler = new DownloadHandlerBuffer();
            return StartCoroutine(Send(request, callback));
        }

        private UnityWebRequest BuildBodyRequest(string method, string path, object body)
        {
            var bytes = Encoding.UTF8.GetBytes(Serializer.Serialize(body));
            var request = new UnityWebRequest(BaseUrl + path, method)
            {
                uploadHandler = new UploadHandlerRaw(bytes),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Content-Type", "application/json");
            return request;
        }

        private IEnumerator Send<T>(UnityWebRequest request, Action<CommonApiResponse<T>> callback)
        {
            foreach (var header in _defaultHeaders)
                request.SetRequestHeader(header.Key, header.Value);

            request.timeout = (int)Timeout;

            using (request)
            {
                yield return request.SendWebRequest();

                var statusCode = (int)request.responseCode;

                if (request.result != UnityWebRequest.Result.Success)
                {
                    callback?.Invoke(CommonApiResponse.Error<T>(request.error, (HttpStatusCode)statusCode));
                    yield break;
                }

                var text = request.downloadHandler.text;

                try
                {
                    callback?.Invoke(Serializer.Deserialize<CommonApiResponse<T>>(text));
                }
                catch (Exception e)
                {
                    // 역직렬화 실패도 예외 대신 Error 응답으로 반환
                    callback?.Invoke(CommonApiResponse.Error<T>($"Deserialize failed: {e.Message}", (HttpStatusCode)statusCode));
                }
            }
        }
    }
}
