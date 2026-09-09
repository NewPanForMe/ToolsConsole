namespace ToolsConsole.Domain.Abstractions;

/// <summary>系统配置读取器：数据库优先，配置文件兜底。</summary>
public interface ISystemConfigProvider
{
    string? GetValue(string key);
}
