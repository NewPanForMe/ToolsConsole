using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ToolsConsole.Application.Common;
using ToolsConsole.Application.Dtos;
using ToolsConsole.Application.Interfaces;
using ToolsConsole.Domain.Abstractions;
using ToolsConsole.Infrastructure.Security;

namespace ToolsConsole.Api.Controllers;

/// <summary>
/// 链接串获取（供其它系统/模块按 ConnName 取链接串）。
/// 按需求该控制器独立且无需身份验证；connName 为空/空白返回 400，未找到或已禁用返回 404。
/// 出于安全考虑，本接口不再直接返回明文链接串，而是使用「分发密钥」(Security:DeliveryKey)
/// 以 AES-256-GCM 加密后返回，并附带解密方式说明（算法/载荷格式/密钥来源）。
/// 仍建议在部署时通过网关/IP 白名单/内网访问控制限制可调用方。
/// </summary>
[ApiController]
[Route("[controller]/[action]")]
[AllowAnonymous]
public sealed class ConnectionStringController : ControllerBase
{
    private readonly IDbConnService _service;
    private readonly IConnectionStringCryptor _deliveryCryptor;

    public ConnectionStringController(
        IDbConnService service,
        [FromKeyedServices(CryptorKeys.Delivery)] IConnectionStringCryptor deliveryCryptor)
    {
        _service = service;
        _deliveryCryptor = deliveryCryptor;
    }

    private const string DecryptInstructionText =
        "算法：AES-256-GCM（密钥 32 字节）。密钥：服务端 Security:DeliveryKey 配置值（32字节密钥的Base64，"
        + "须由调用方与服务端提前线下共享，不随本接口下发）。载荷：Base64(nonce[12]+密文+tag[16])，nonce 每次随机。"
        + "C#/Node.js 等语言解密示例见项目 README.md「取串接口解密」章节。";

    /// <summary>
    /// GET /ConnectionString/GetByConnName?connName=xxx
    /// 返回：{ code:0, data: { connName, dbType, connectionStringEncrypted, algorithm, keySource, payloadLayout, decryptInstructions } }
    /// 明文链接串不会出现在响应中。
    /// </summary>
    [HttpGet]
    [ActionName("GetByConnName")]
    public async Task<IActionResult> GetByConnNameAsync(
        [FromQuery] string? connName,
        CancellationToken cancellationToken)
    {
        var resolved = await _service.GetConnectionStringByConnNameAsync(connName, cancellationToken);

        var delivery = new DbConnDtos.ConnectionStringDelivery
        {
            ConnName = resolved.ConnName,
            DbType = resolved.DbType,
            ConnectionStringEncrypted = _deliveryCryptor.Encrypt(resolved.ConnectionString),
            DecryptInstructions = DecryptInstructionText,
        };

        return Ok(ApiResult<DbConnDtos.ConnectionStringDelivery>.Ok(delivery));
    }
}
