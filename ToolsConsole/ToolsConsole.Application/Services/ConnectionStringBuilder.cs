using System.Text;
using ToolsConsole.Application.Common;

namespace ToolsConsole.Application.Services;

/// <summary>把数据库连接各元素组装为链接串（当前仅实现 PostgreSQL；其它类型抛出业务异常提示）。</summary>
public static class ConnectionStringBuilder
{
    private const string PgKeywordSslMode = "SSL Mode";

    /// <summary>组装链接串。options 形如 "SSL Mode=Disable;Timeout=15"，可为 null。</summary>
    public static string Build(
        string dbType,
        string host,
        int port,
        string database,
        string username,
        string password,
        string? options)
    {
        var type = (dbType ?? string.Empty).Trim();
        return type.ToLowerInvariant() switch
        {
            "postgresql" or "pgsql" or "postgres" => BuildPostgreSql(host, port, database, username, password, options),
            _ => throw ApiException.BadRequest($"暂不支持组装 [{type}] 的链接串（当前仅支持 PostgreSQL）"),
        };
    }

    private static string BuildPostgreSql(
        string host,
        int port,
        string database,
        string username,
        string password,
        string? options)
    {
        var sb = new StringBuilder(256);
        sb.Append("Host=").Append(host)
          .Append(";Port=").Append(port)
          .Append(";Database=").Append(database)
          .Append(";Username=").Append(username)
          .Append(";Password=").Append(password);

        var extra = (options ?? string.Empty).Trim().TrimEnd(';');
        if (extra.Length > 0)
        {
            sb.Append(';').Append(extra);
        }

        // 未显式指定 SSL Mode 时按本机环境的常见方式处理（可经 options 覆盖）。
        if (extra.Length == 0 ||
            !extra.Contains(PgKeywordSslMode, StringComparison.OrdinalIgnoreCase))
        {
            sb.Append(";SSL Mode=Disable");
        }

        return sb.ToString();
    }
}
