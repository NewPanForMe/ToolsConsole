using ToolsConsole.Domain.Common;

namespace ToolsConsole.Domain.Entities;

/// <summary>
/// 数据库链接字串配置：链接串的各组成元素分列存储，
/// 其中 Password 由调用方加密后传入（PasswordEncrypted 列保存密文），永不落明文。
/// </summary>
public sealed class DbConn : EntityBase
{
    public const int StatusEnabled = 1;
    public const int StatusDisabled = 0;
    public const int DefaultPort = 5432;

    private DbConn()
    {
        // 供 EF Core 物化使用
    }

    /// <summary>连接名称，全系统唯一，获取链接串的唯一键。</summary>
    public string ConnName { get; private set; } = string.Empty;

    /// <summary>数据库类型（见 <see cref="DbTypes"/>，当前支持 PostgreSQL 组装）。</summary>
    public string DbType { get; private set; } = DbTypes.PostgreSQL;

    public string Host { get; private set; } = string.Empty;

    public int Port { get; private set; } = DefaultPort;

    public string Database { get; private set; } = string.Empty;

    public string Username { get; private set; } = string.Empty;

    /// <summary>AES-GCM 密文（Base64）。空串表示该连接无密码。</summary>
    public string PasswordEncrypted { get; private set; } = string.Empty;

    /// <summary>附加连接参数，如 "SSL Mode=Disable"、"Timeout=15"。</summary>
    public string? Options { get; private set; }

    public string? Remark { get; private set; }

    /// <summary>1-启用 0-禁用。</summary>
    public int Status { get; private set; } = StatusEnabled;

    public static DbConn Create(
        string connName,
        string dbType,
        string host,
        int port,
        string database,
        string username,
        string encryptedPassword,
        string? options = null,
        string? remark = null)
    {
        var entity = new DbConn();
        entity.SetBase(connName, dbType, host, port, database, username);
        entity.PasswordEncrypted = encryptedPassword ?? string.Empty;
        entity.Options = options;
        entity.Remark = remark;
        entity.Status = StatusEnabled;
        entity.MarkUpdated();
        return entity;
    }

    /// <summary>
    /// 全量更新。
    /// </summary>
    /// <param name="encryptedPassword">非 null 时更新密码密文；为 null 表示保持原密码不变。</param>
    public void Update(
        string connName,
        string dbType,
        string host,
        int port,
        string database,
        string username,
        string? encryptedPassword,
        string? options,
        string? remark,
        int status)
    {
        SetBase(connName, dbType, host, port, database, username);
        if (encryptedPassword is not null)
        {
            PasswordEncrypted = encryptedPassword;
        }

        Options = options;
        Remark = remark;
        Status = status;
        MarkUpdated();
    }

    private void SetBase(string connName, string dbType, string host, int port, string database, string username)
    {
        if (string.IsNullOrWhiteSpace(connName))
        {
            throw new ArgumentException("ConnName 不能为空", nameof(connName));
        }

        if (string.IsNullOrWhiteSpace(host))
        {
            throw new ArgumentException("Host 不能为空", nameof(host));
        }

        ConnName = connName.Trim();
        DbType = string.IsNullOrWhiteSpace(dbType) ? DbTypes.PostgreSQL : dbType.Trim();
        Host = host.Trim();
        Port = port <= 0 ? DefaultPort : port;
        Database = (database ?? string.Empty).Trim();
        Username = (username ?? string.Empty).Trim();
    }
}
