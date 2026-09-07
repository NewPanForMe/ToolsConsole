using ToolsConsole.Application.Dtos;

namespace ToolsConsole.Application.Interfaces;

/// <summary>数据库链接字串维护服务。</summary>
public interface IDbConnService
{
    Task<PagedResult<DbConnDtos.DbConnDto>> GetPageAsync(
        string? keyword,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<DbConnDtos.DbConnDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<DbConnDtos.DbConnDto> CreateAsync(
        DbConnDtos.DbConnCreateRequest request,
        CancellationToken cancellationToken = default);

    Task<DbConnDtos.DbConnDto> UpdateAsync(
        long id,
        DbConnDtos.DbConnUpdateRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 按 ConnName 解密并组装链接串（仅在服务进程内使用，由 Api 层加密后出网）。
    /// connName 为空/空白 → 400；未找到或已禁用 → 404；不支持的 DbType → 400。
    /// </summary>
    Task<DbConnDtos.ResolvedConnectionString> GetConnectionStringByConnNameAsync(
        string? connName,
        CancellationToken cancellationToken = default);
}
