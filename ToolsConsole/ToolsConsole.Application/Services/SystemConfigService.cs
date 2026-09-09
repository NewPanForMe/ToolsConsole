using ToolsConsole.Application.Common;
using ToolsConsole.Application.Dtos;
using ToolsConsole.Application.Interfaces;
using ToolsConsole.Domain.Abstractions;
using ToolsConsole.Domain.Entities;

namespace ToolsConsole.Application.Services;

/// <inheritdoc cref="ISystemConfigService" />
public sealed class SystemConfigService : ISystemConfigService
{
    private readonly IRepository<SystemConfig> _configs;

    public SystemConfigService(IRepository<SystemConfig> configs)
    {
        _configs = configs;
    }

    public async Task<PagedResult<SystemConfigDtos.SystemConfigDto>> GetPageAsync(
        string? keyword,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageIndex = Math.Max(1, pageIndex);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var kw = (keyword ?? string.Empty).Trim();
        var predicate = kw.Length == 0
            ? null
            : (System.Linq.Expressions.Expression<Func<SystemConfig, bool>>)(c =>
                c.ConfigKey.Contains(kw) || (c.Remark != null && c.Remark.Contains(kw)));

        var total = await _configs.CountAsync(predicate, cancellationToken);
        var items = await _configs.GetListAsync(
            predicate,
            (pageIndex - 1) * pageSize,
            pageSize,
            q => q.OrderBy(c => c.ConfigKey),
            cancellationToken);

        return new PagedResult<SystemConfigDtos.SystemConfigDto>
        {
            List = items.Select(ToDto).ToList(),
            Total = total,
            PageIndex = pageIndex,
            PageSize = pageSize,
        };
    }

    public async Task<SystemConfigDtos.SystemConfigDto?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        var entity = await _configs.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<SystemConfigDtos.SystemConfigDto?> GetByKeyAsync(
        string configKey,
        CancellationToken cancellationToken = default)
    {
        var key = NormalizeKey(configKey);
        var entity = await _configs.FirstOrDefaultAsync(c => c.ConfigKey == key, cancellationToken);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<SystemConfigDtos.SystemConfigDto> CreateAsync(
        SystemConfigDtos.CreateSystemConfigRequest request,
        CancellationToken cancellationToken = default)
    {
        var key = NormalizeKey(request.ConfigKey);
        if (await _configs.AnyAsync(c => c.ConfigKey == key, cancellationToken))
        {
            throw ApiException.Conflict($"配置项 [{key}] 已存在");
        }

        var entity = SystemConfig.Create(key, request.ConfigValue ?? string.Empty, NormalizeOptional(request.Remark));
        await _configs.AddAsync(entity, cancellationToken);
        return ToDto(entity);
    }

    public async Task<SystemConfigDtos.SystemConfigDto> UpdateAsync(
        long id,
        SystemConfigDtos.UpdateSystemConfigRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = await _configs.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            throw ApiException.NotFound($"系统配置不存在（id={id}）");
        }

        entity.UpdateValue(request.ConfigValue ?? string.Empty, NormalizeOptional(request.Remark));
        await _configs.UpdateAsync(entity, cancellationToken);
        return ToDto(entity);
    }

    public async Task<SystemConfigDtos.SystemConfigDto> SaveByKeyAsync(
        SystemConfigDtos.SaveSystemConfigByKeyRequest request,
        CancellationToken cancellationToken = default)
    {
        var key = NormalizeKey(request.ConfigKey);
        var entity = await _configs.FirstOrDefaultAsync(c => c.ConfigKey == key, cancellationToken);

        if (entity is null)
        {
            entity = SystemConfig.Create(key, request.ConfigValue ?? string.Empty, NormalizeOptional(request.Remark));
            await _configs.AddAsync(entity, cancellationToken);
            return ToDto(entity);
        }

        entity.UpdateValue(request.ConfigValue ?? string.Empty, NormalizeOptional(request.Remark));
        await _configs.UpdateAsync(entity, cancellationToken);
        return ToDto(entity);
    }

    private static string NormalizeKey(string? configKey)
    {
        var key = (configKey ?? string.Empty).Trim();
        if (key.Length == 0)
        {
            throw ApiException.BadRequest("配置 Key 不能为空");
        }

        if (key.Length > 120)
        {
            throw ApiException.BadRequest("配置 Key 长度不能超过 120 位");
        }

        return key;
    }

    private static string? NormalizeOptional(string? value)
    {
        var trimmed = (value ?? string.Empty).Trim();
        return trimmed.Length == 0 ? null : trimmed;
    }

    private static SystemConfigDtos.SystemConfigDto ToDto(SystemConfig entity)
        => new()
        {
            Id = entity.Id,
            ConfigKey = entity.ConfigKey,
            ConfigValue = entity.ConfigValue,
            Remark = entity.Remark,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
}
