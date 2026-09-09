using ToolsConsole.Domain.Common;

namespace ToolsConsole.Domain.Entities;

/// <summary>系统级配置项，用于保存 JWT、加密等运行密钥。</summary>
public sealed class SystemConfig : EntityBase
{
    private SystemConfig()
    {
        // 供 EF Core 物化使用
    }

    public string ConfigKey { get; private set; } = string.Empty;

    public string ConfigValue { get; private set; } = string.Empty;

    public string? Remark { get; private set; }

    public static SystemConfig Create(string configKey, string configValue, string? remark = null)
    {
        if (string.IsNullOrWhiteSpace(configKey))
        {
            throw new ArgumentException("配置 Key 不能为空", nameof(configKey));
        }

        return new SystemConfig
        {
            ConfigKey = configKey.Trim(),
            ConfigValue = configValue ?? string.Empty,
            Remark = string.IsNullOrWhiteSpace(remark) ? null : remark.Trim(),
        };
    }

    public void UpdateValue(string configValue, string? remark = null)
    {
        ConfigValue = configValue ?? string.Empty;
        Remark = string.IsNullOrWhiteSpace(remark) ? Remark : remark.Trim();
        MarkUpdated();
    }
}
