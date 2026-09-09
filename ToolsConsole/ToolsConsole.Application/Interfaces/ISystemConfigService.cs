using ToolsConsole.Application.Dtos;

namespace ToolsConsole.Application.Interfaces;

/// <summary>系统配置管理服务。</summary>
public interface ISystemConfigService
{
    Task<PagedResult<SystemConfigDtos.SystemConfigDto>> GetPageAsync(
        string? keyword,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<SystemConfigDtos.SystemConfigDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<SystemConfigDtos.SystemConfigDto?> GetByKeyAsync(string configKey, CancellationToken cancellationToken = default);

    Task<SystemConfigDtos.SystemConfigDto> CreateAsync(
        SystemConfigDtos.CreateSystemConfigRequest request,
        CancellationToken cancellationToken = default);

    Task<SystemConfigDtos.SystemConfigDto> UpdateAsync(
        long id,
        SystemConfigDtos.UpdateSystemConfigRequest request,
        CancellationToken cancellationToken = default);

    Task<SystemConfigDtos.SystemConfigDto> SaveByKeyAsync(
        SystemConfigDtos.SaveSystemConfigByKeyRequest request,
        CancellationToken cancellationToken = default);
}
