using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ToolsConsole.Domain.Abstractions;
using ToolsConsole.Infrastructure.Data;

namespace ToolsConsole.Infrastructure.Configuration;

/// <summary>从 system_configs 读取系统 Key，缺失时回退到 appsettings。</summary>
public sealed class DbSystemConfigProvider : ISystemConfigProvider
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DbSystemConfigProvider> _logger;

    public DbSystemConfigProvider(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<DbSystemConfigProvider> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public string? GetValue(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return null;
        }

        try
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var dbValue = db.SystemConfigs
                .AsNoTracking()
                .Where(c => c.ConfigKey == key)
                .Select(c => c.ConfigValue)
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(dbValue))
            {
                return dbValue;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "读取数据库系统配置失败，使用配置文件兜底。Key={Key}", key);
        }

        return _configuration[key];
    }
}
