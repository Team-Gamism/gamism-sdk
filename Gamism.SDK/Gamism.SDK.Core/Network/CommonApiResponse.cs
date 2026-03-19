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
    /// 데이터 없는 응답에 사용하는 비제네릭 CommonApiResponse.
    /// CommonApiResponse&lt;EmptyResponse&gt;의 편의 타입으로, 팩토리 메서드도 포함한다.
    /// </summary>
    public class CommonApiResponse : CommonApiResponse<EmptyResponse>
    {
        internal CommonApiResponse(string status, int code, string message)
            : base(status, code, message) { }

        public static CommonApiResponse Success(string message) =>
            new CommonApiResponse(HttpStatusCode.OK.ToString().ToUpper(), (int)HttpStatusCode.OK, message);

        public static CommonApiResponse<T> Success<T>(string message, T data) =>
            new CommonApiResponse<T>(HttpStatusCode.OK.ToString().ToUpper(), (int)HttpStatusCode.OK, message, data);

        public static CommonApiResponse Created(string message) =>
            new CommonApiResponse(HttpStatusCode.Created.ToString().ToUpper(), (int)HttpStatusCode.Created, message);

        public static CommonApiResponse<T> Created<T>(string message, T data) =>
            new CommonApiResponse<T>(HttpStatusCode.Created.ToString().ToUpper(), (int)HttpStatusCode.Created, message, data);

        public static CommonApiResponse Error(string message, HttpStatusCode status) =>
            new CommonApiResponse(status.ToString().ToUpper(), (int)status, message);

        internal static CommonApiResponse<T> Error<T>(string message, HttpStatusCode status) =>
            new CommonApiResponse<T>(status.ToString().ToUpper(), (int)status, message);
    }
}
