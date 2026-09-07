using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ToolsConsole.Application.Common;

namespace ToolsConsole.Api.Middlewares;

/// <summary>全局异常处理：ApiException → 对应状态码；其它异常 → 500（不向外泄露细节）。</summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ApiException ex)
        {
            _logger.LogWarning("业务异常: {Message}", ex.Message);
            await WriteErrorAsync(context, ex.StatusCode, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "未处理异常");
            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, 500, "服务器内部错误");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, int httpStatusCode, int code, string message)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.StatusCode = httpStatusCode;
        context.Response.ContentType = "application/json; charset=utf-8";
        await context.Response.WriteAsJsonAsync(new { code, message });
    }
}
