using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolsConsole.Api.Security;
using ToolsConsole.Application.Common;
using ToolsConsole.Application.Dtos;
using ToolsConsole.Application.Interfaces;
using ToolsConsole.Domain.Abstractions;

namespace ToolsConsole.Api.Controllers;

/// <summary>认证（登录）。</summary>
[ApiController]
[Route("[controller]/[action]")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly JwtTokenGenerator _tokenGenerator;
    private readonly ITransportPasswordDecryptor _transportDecryptor;

    public AuthController(
        IAuthService authService,
        JwtTokenGenerator tokenGenerator,
        ITransportPasswordDecryptor transportDecryptor)
    {
        _authService = authService;
        _tokenGenerator = tokenGenerator;
        _transportDecryptor = transportDecryptor;
    }

    /// <summary>
    /// POST /Auth/Login 匿名登录，返回 JWT。
    /// Password 为前端 AES-256-CBC 加密后的密文，服务端解密后校验。
    /// </summary>
    [AllowAnonymous]
    [HttpPost]
    [ActionName("Login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var plainPassword = DecryptPassword(request.Password);
        var login = await _authService.LoginAsync(
            new LoginRequest { UserName = request.UserName, Password = plainPassword },
            cancellationToken);
        var token = _tokenGenerator.CreateToken(login);
        var response = new LoginResponse
        {
            Token = token,
            UserName = login.UserName,
            DisplayName = login.DisplayName,
            ExpiresIn = _tokenGenerator.ExpireSeconds,
        };
        return Ok(ApiResult<LoginResponse>.Ok(response, "登录成功"));
    }

    /// <summary>GET /Auth/Info 当前登录用户信息（需带 Bearer Token）。</summary>
    [Authorize]
    [HttpGet]
    [ActionName("Info")]
    public IActionResult Info()
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty;
        var displayName = User.FindFirstValue("displayName") ?? userName;
        return Ok(ApiResult<object>.Ok(new
        {
            userId = long.TryParse(userIdText, out var id) ? id : 0,
            userName,
            displayName,
        }));
    }

    private string DecryptPassword(string? encrypted)
    {
        try
        {
            return _transportDecryptor.Decrypt(encrypted ?? string.Empty);
        }
        catch (CryptographicException ex)
        {
            throw ApiException.BadRequest($"密码解密失败：{ex.Message}（请确认前后端 TransportKey/TransportIv 一致）");
        }
    }
}
