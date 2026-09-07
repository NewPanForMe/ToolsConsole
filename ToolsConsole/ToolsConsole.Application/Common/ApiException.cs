namespace ToolsConsole.Application.Common;

/// <summary>业务异常：由 Api 层统一转换为对应 HTTP 状态码的 { code, message } 响应。</summary>
public sealed class ApiException : Exception
{
    public ApiException(int statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    }

    /// <summary>建议的 HTTP 状态码。</summary>
    public int StatusCode { get; }

    public static ApiException BadRequest(string message) => new(400, message);

    public static ApiException Unauthorized(string message) => new(401, message);

    public static ApiException NotFound(string message) => new(404, message);

    public static ApiException Conflict(string message) => new(409, message);
}
