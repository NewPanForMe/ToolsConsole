namespace ToolsConsole.Application.Dtos;

/// <summary>登录请求。</summary>
public sealed class LoginRequest
{
    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}

/// <summary>登录成功后由 AuthService 返回的身份信息（Token 由 Api 层签发）。</summary>
public sealed class LoginResult
{
    public long UserId { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;
}

/// <summary>登录接口最终响应。</summary>
public sealed class LoginResponse
{
    public string Token { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Token 有效期（秒）。</summary>
    public int ExpiresIn { get; set; }
}
