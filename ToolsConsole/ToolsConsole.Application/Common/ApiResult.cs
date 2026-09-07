namespace ToolsConsole.Application.Common;

/// <summary>统一响应封装：Code=0 表示成功，其余为错误码（与 HTTP 状态码保持一致）。</summary>
public sealed class ApiResult<T>
{
    public int Code { get; set; }

    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }

    public static ApiResult<T> Ok(T? data = default, string message = "操作成功")
        => new() { Code = 0, Message = message, Data = data };

    public static ApiResult<T> Fail(int code, string message)
        => new() { Code = code, Message = message };
}
