using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolsConsole.Application.Common;
using ToolsConsole.Application.Dtos;
using ToolsConsole.Application.Interfaces;

namespace ToolsConsole.Api.Controllers;

/// <summary>系统配置管理（需登录）。</summary>
[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public sealed class SystemConfigController : ControllerBase
{
    private readonly ISystemConfigService _service;

    public SystemConfigController(ISystemConfigService service)
    {
        _service = service;
    }

    /// <summary>GET /SystemConfig/GetPage?keyword=&amp;pageIndex=1&amp;pageSize=20</summary>
    [HttpGet]
    [ActionName("GetPage")]
    public async Task<IActionResult> GetPageAsync(
        [FromQuery] string? keyword,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.GetPageAsync(keyword, pageIndex, pageSize, cancellationToken);
        return Ok(ApiResult<PagedResult<SystemConfigDtos.SystemConfigDto>>.Ok(result));
    }

    /// <summary>GET /SystemConfig/GetById/5</summary>
    [HttpGet("{id:long}")]
    [ActionName("GetById")]
    public async Task<IActionResult> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetByIdAsync(id, cancellationToken);
        return dto is null
            ? NotFound(ApiResult<object>.Fail(404, $"系统配置不存在（id={id}）"))
            : Ok(ApiResult<SystemConfigDtos.SystemConfigDto>.Ok(dto));
    }

    /// <summary>GET /SystemConfig/GetByKey?configKey=SiteConfig:TechStack</summary>
    [HttpGet]
    [ActionName("GetByKey")]
    public async Task<IActionResult> GetByKeyAsync([FromQuery] string configKey, CancellationToken cancellationToken)
    {
        var dto = await _service.GetByKeyAsync(configKey, cancellationToken);
        return dto is null
            ? NotFound(ApiResult<object>.Fail(404, $"系统配置不存在（key={configKey}）"))
            : Ok(ApiResult<SystemConfigDtos.SystemConfigDto>.Ok(dto));
    }

    /// <summary>POST /SystemConfig/Create</summary>
    [HttpPost]
    [ActionName("Create")]
    public async Task<IActionResult> CreateAsync(
        [FromBody] SystemConfigDtos.CreateSystemConfigRequest request,
        CancellationToken cancellationToken)
    {
        var dto = await _service.CreateAsync(request, cancellationToken);
        return Ok(ApiResult<SystemConfigDtos.SystemConfigDto>.Ok(dto, "新增成功"));
    }

    /// <summary>POST /SystemConfig/Update/5</summary>
    [HttpPost("{id:long}")]
    [ActionName("Update")]
    public async Task<IActionResult> UpdateAsync(
        long id,
        [FromBody] SystemConfigDtos.UpdateSystemConfigRequest request,
        CancellationToken cancellationToken)
    {
        var dto = await _service.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResult<SystemConfigDtos.SystemConfigDto>.Ok(dto, "保存成功"));
    }

    /// <summary>POST /SystemConfig/SaveByKey</summary>
    [HttpPost]
    [ActionName("SaveByKey")]
    public async Task<IActionResult> SaveByKeyAsync(
        [FromBody] SystemConfigDtos.SaveSystemConfigByKeyRequest request,
        CancellationToken cancellationToken)
    {
        var dto = await _service.SaveByKeyAsync(request, cancellationToken);
        return Ok(ApiResult<SystemConfigDtos.SystemConfigDto>.Ok(dto, "保存成功"));
    }
}
