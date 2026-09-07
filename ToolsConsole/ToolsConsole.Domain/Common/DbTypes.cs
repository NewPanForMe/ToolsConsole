namespace ToolsConsole.Domain.Common;

/// <summary>支持的数据库类型常量（当前仅 PostgreSQL 可组装链接串，其余为预留扩展）。</summary>
public static class DbTypes
{
    public const string PostgreSQL = "PostgreSQL";
    public const string SqlServer = "SqlServer";
    public const string MySql = "MySql";
    public const string Oracle = "Oracle";
}
