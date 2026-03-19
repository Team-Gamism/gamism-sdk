using System.Net;

namespace Gamism.SDK.Core.Network
{
    /// <summary>
    /// ApiResponseWrapperFilter에서 이미 래핑된 응답을 식별하기 위한 마커 인터페이스.
    /// </summary>
    public interface ICommonApiResponse
    {
        int Code { get; }
    }

    /// <summary>
    /// 서버와 클라이언트가 공유하는 공통 API 응답 포맷.
    /// </summary>
    public class CommonApiResponse<T> : ICommonApiResponse
    {
        public string Status { get; }
        public int Code { get; }
        public string Message { get; }
        public T Data { get; }

        // JSON 역직렬화 및 팩토리용 생성자
        public CommonApiResponse(string status, int code, string message, T data = default)
        {
            Status = status;
            Code = code;
            Message = message;
            Data = data;
        }
    }

    /// <summary>
    /// 데이터 없는 응답에 사용하는 빈 응답 타입.
    /// </summary>
    public sealed class EmptyResponse { }

    /// <summary>
    /// CommonApiResponse 팩토리 메서드 모음.
    /// </summary>
    public static class CommonApiResponse
    {
        public static CommonApiResponse<EmptyResponse> Success(string message) =>
            Build<EmptyResponse>(HttpStatusCode.OK, message);

        public static CommonApiResponse<T> Success<T>(string message, T data) =>
            Build(HttpStatusCode.OK, message, data);

        public static CommonApiResponse<EmptyResponse> Created(string message) =>
            Build<EmptyResponse>(HttpStatusCode.Created, message);

        public static CommonApiResponse<T> Created<T>(string message, T data) =>
            Build(HttpStatusCode.Created, message, data);

        public static CommonApiResponse<EmptyResponse> Error(string message, HttpStatusCode status) =>
            Build<EmptyResponse>(status, message);

        internal static CommonApiResponse<T> Error<T>(string message, HttpStatusCode status) =>
            Build<T>(status, message);

        private static CommonApiResponse<T> Build<T>(HttpStatusCode status, string message, T data = default) =>
            new CommonApiResponse<T>(status.ToString().ToUpper(), (int)status, message, data);
    }
}
