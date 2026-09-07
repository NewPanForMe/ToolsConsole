using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToolsConsole.Application.Common;
using ToolsConsole.Application.Dtos;
using ToolsConsole.Application.Interfaces;

namespace ToolsConsole.Api.Controllers;

/// <summary>数据库链接字串维护（需登录）。</summary>
[ApiController]
[Route("[controller]/[action]")]
[Authorize]
public sealed class DbConnController : ControllerBase
{
    private readonly IDbConnService _service;

    public DbConnController(IDbConnService service)
    {
        _service = service;
    }

    /// <summary>GET /DbConn/GetPage?keyword=&amp;pageIndex=1&amp;pageSize=20</summary>
    [HttpGet]
    [ActionName("GetPage")]
    public async Task<IActionResult> GetPageAsync(
        [FromQuery] string? keyword,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.GetPageAsync(keyword, pageIndex, pageSize, cancellationToken);
        return Ok(ApiResult<PagedResult<DbConnDtos.DbConnDto>>.Ok(result));
    }

    /// <summary>GET /DbConn/GetById/5</summary>
    [HttpGet("{id:long}")]
    [ActionName("GetById")]
    public async Task<IActionResult> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var dto = await _service.GetByIdAsync(id, cancellationToken);
        return dto is null
            ? NotFound(ApiResult<object>.Fail(404, $"连接配置不存在（id={id}）"))
            : Ok(ApiResult<DbConnDtos.DbConnDto>.Ok(dto));
    }

    /// <summary>POST /DbConn/Create</summary>
    [HttpPost]
    [ActionName("Create")]
    public async Task<IActionResult> CreateAsync(
        [FromBody] DbConnDtos.DbConnCreateRequest request,
        CancellationToken cancellationToken)
    {
        var dto = await _service.CreateAsync(request, cancellationToken);
        return Ok(ApiResult<DbConnDtos.DbConnDto>.Ok(dto, "新增成功"));
    }

    /// <summary>POST /DbConn/Update/5</summary>
    [HttpPost("{id:long}")]
    [ActionName("Update")]
    public async Task<IActionResult> UpdateAsync(
        long id,
        [FromBody] DbConnDtos.DbConnUpdateRequest request,
        CancellationToken cancellationToken)
    {
        var dto = await _service.UpdateAsync(id, request, cancellationToken);
        return Ok(ApiResult<DbConnDtos.DbConnDto>.Ok(dto, "保存成功"));
    }

    /// <summary>DELETE /DbConn/Delete/5</summary>
    [HttpDelete("{id:long}")]
    [ActionName("Delete")]
    public async Task<IActionResult> DeleteAsync(long id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return Ok(ApiResult<object>.Ok(null, "删除成功"));
    }
}
