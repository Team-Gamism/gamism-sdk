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
    public class ApiManager : MonoSingleton<ApiManager>
    {
        public string BaseUrl { get; set; } = string.Empty;
        public float Timeout { get; set; } = 30f;
        public IJsonSerializer Serializer { get; set; } = new NewtonsoftJsonSerializer();

        private readonly Dictionary<string, string> _defaultHeaders = new Dictionary<string, string>();

        public void SetDefaultHeader(string key, string value) => _defaultHeaders[key] = value;
        public void RemoveDefaultHeader(string key) => _defaultHeaders.Remove(key);

        public Coroutine Get<T>(string path, Action<CommonApiResponse<T>> callback)
        {
            var request = UnityWebRequest.Get(BaseUrl + path);
            return StartCoroutine(Send(request, callback));
        }

        public Coroutine Post<T>(string path, object body, Action<CommonApiResponse<T>> callback)
        {
            var request = BuildBodyRequest("POST", path, body);
            return StartCoroutine(Send(request, callback));
        }

        public Coroutine Put<T>(string path, object body, Action<CommonApiResponse<T>> callback)
        {
            var request = BuildBodyRequest("PUT", path, body);
            return StartCoroutine(Send(request, callback));
        }

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
                    callback?.Invoke(CommonApiResponse.Error<T>($"Deserialize failed: {e.Message}", (HttpStatusCode)statusCode));
                }
            }
        }
    }
}
