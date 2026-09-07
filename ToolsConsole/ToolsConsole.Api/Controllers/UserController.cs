using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolsConsole.Application.Common;
using ToolsConsole.Application.Dtos;
using ToolsConsole.Application.Interfaces;
using ToolsConsole.Domain.Abstractions;

namespace ToolsConsole.Api.Controllers;

/// <summary>用户管理（需登录）。</summary>
[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public sealed class UserController : ControllerBase
{
    private readonly IUserService _service;
    private readonly ITransportPasswordDecryptor _transportDecryptor;

    public UserController(IUserService service, ITransportPasswordDecryptor transportDecryptor)
    {
        _service = service;
        _transportDecryptor = transportDecryptor;
    }

    /// <summary>GET /User/GetPage?keyword=&amp;pageIndex=1&amp;pageSize=20</summary>
    [HttpGet]
    [ActionName("GetPage")]
    public async Task<IActionResult> GetPageAsync(
        [FromQuery] string? keyword,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.GetPageAsync(keyword, pageIndex, pageSize, cancellationToken);
        return Ok(ApiResult<PagedResult<UserDtos.UserDto>>.Ok(result));
    }

    /// <summary>GET /User/GetById/5</summary>
    [HttpGet("{id:long}")]
    [ActionName("GetById")]
    public async Task<IActionResult> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetByIdAsync(id, cancellationToken);
        return dto is null
            ? NotFound(ApiResult<object>.Fail(404, $"用户不存在（id={id}）"))
            : Ok(ApiResult<UserDtos.UserDto>.Ok(dto));
    }

    /// <summary>POST /User/Create（password 为 AES-256-CBC 密文，服务端解密后哈希落库）</summary>
    [HttpPost]
    [ActionName("Create")]
    public async Task<IActionResult> CreateAsync(
        [FromBody] UserDtos.CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        request.Password = DecryptPassword(request.Password);
        var dto = await _service.CreateAsync(request, cancellationToken);
        return Ok(ApiResult<UserDtos.UserDto>.Ok(dto, "新增成功"));
    }

    /// <summary>PUT /User/Update/5</summary>
    [HttpPut("{id:long}")]
    [ActionName("Update")]
    public async Task<IActionResult> UpdateAsync(
        long id,
        [FromBody] UserDtos.UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var dto = await _service.UpdateAsync(id, request, BuildOperator(), cancellationToken);
        return Ok(ApiResult<UserDtos.UserDto>.Ok(dto, "保存成功"));
    }

    /// <summary>POST /User/ResetPassword/5（password 为 AES-256-CBC 密文，服务端解密后哈希）</summary>
    [HttpPost("{id:long}")]
    [ActionName("ResetPassword")]
    public async Task<IActionResult> ResetPasswordAsync(
        long id,
        [FromBody] UserDtos.ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        request.Password = DecryptPassword(request.Password);
        await _service.ResetPasswordAsync(id, request, cancellationToken);
        return Ok(ApiResult<object>.Ok(null, "密码已重置"));
    }

    /// <summary>DELETE /User/Delete/5</summary>
    [HttpDelete("{id:long}")]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, BuildOperator(), cancellationToken);
        return Ok(ApiResult<object>.Ok(null, "删除成功"));
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

    private UserDtos.OperatorInfo BuildOperator()
    {
        var userIdText = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return new UserDtos.OperatorInfo
        {
            UserId = long.TryParse(userIdText, out var userId) ? userId : 0,
            UserName = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
        };
    }
}
