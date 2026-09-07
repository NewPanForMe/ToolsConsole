using ToolsConsole.Application.Dtos;

namespace ToolsConsole.Application.Interfaces;

/// <summary>认证服务。</summary>
public interface IAuthService
{
    /// <summary>校验用户名密码并返回登录用户身份；失败抛出 <see cref="Common.ApiException"/>（401）。</summary>
    Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
