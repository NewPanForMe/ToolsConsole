namespace ToolsConsole.Application.Dtos;

/// <summary>系统配置 DTO。</summary>
public static class SystemConfigDtos
{
    /// <summary>系统配置展示/编辑回显 DTO。</summary>
    public sealed class SystemConfigDto
    {
        public long Id { get; set; }

        public string ConfigKey { get; set; } = string.Empty;

        public string ConfigValue { get; set; } = string.Empty;

        public string? Remark { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>新增系统配置。</summary>
    public sealed class CreateSystemConfigRequest
    {
        public string ConfigKey { get; set; } = string.Empty;

        public string ConfigValue { get; set; } = string.Empty;

        public string? Remark { get; set; }
    }

    /// <summary>编辑系统配置。ConfigKey 不允许修改。</summary>
    public sealed class UpdateSystemConfigRequest
    {
        public string ConfigValue { get; set; } = string.Empty;

        public string? Remark { get; set; }
    }
}
