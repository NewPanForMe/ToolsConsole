namespace ToolsConsole.Domain.Common;

/// <summary>实体基类：自增主键与审计时间（使用本机时间，遵循 CST 约定）。</summary>
public abstract class EntityBase
{
    public long Id { get; protected set; }

    public DateTime CreatedAt { get; protected set; } = DateTime.Now;

    public DateTime UpdatedAt { get; protected set; } = DateTime.Now;

    /// <summary>标记实体已变更（刷新 UpdatedAt）。</summary>
    public void MarkUpdated() => UpdatedAt = DateTime.Now;
}
