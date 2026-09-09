using System.Text.Json.Serialization;

namespace ToolsConsole.Application.Dtos;

/// <summary>连接配置“修改密码时传此值表示保持原密码不变”。</summary>
public static class DbConnDtos
{
    public const string PasswordUnchanged = "******";

    /// <summary>连接配置展示/编辑回显 DTO（不包含任何密码明文/密文）。</summary>
    public sealed class DbConnDto
    {
        public long Id { get; set; }

        public string ConnName { get; set; } = string.Empty;

        public string DbType { get; set; } = "PostgreSQL";

        public string Host { get; set; } = string.Empty;

        public int Port { get; set; } = 5432;

        public string Database { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        public string? Options { get; set; }

        public string? Remark { get; set; }

        public int Status { get; set; } = 1;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>新增连接配置。</summary>
    public sealed class DbConnCreateRequest
    {
        public string ConnName { get; set; } = string.Empty;

        public string DbType { get; set; } = "PostgreSQL";

        public string Host { get; set; } = string.Empty;

        public int Port { get; set; } = 5432;

        public string Database { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        /// <summary>明文密码（服务端加密后落库）；null/空串表示无密码。</summary>
        public string? Password { get; set; }

        public string? Options { get; set; }

        public string? Remark { get; set; }

        public int Status { get; set; } = 1;
    }

    /// <summary>编辑连接配置。Password 语义：null 或 ****** = 不修改；空串 = 清除密码；其它 = 重新加密。</summary>
    public sealed class DbConnUpdateRequest
    {
        public string ConnName { get; set; } = string.Empty;

        public string DbType { get; set; } = "PostgreSQL";

        public string Host { get; set; } = string.Empty;

        public int Port { get; set; } = 5432;

        public string Database { get; set; } = string.Empty;

        public string Username { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Password { get; set; }

        public string? Options { get; set; }

        public string? Remark { get; set; }

        public int Status { get; set; } = 1;
    }

    /// <summary>服务端内部解析结果（仅存在于服务进程内，不出网，避免明文暴露）。</summary>
    public sealed class ResolvedConnectionString
    {
        public string ConnName { get; set; } = string.Empty;

        public string DbType { get; set; } = string.Empty;

        public string ConnectionString { get; set; } = string.Empty;
    }

    /// <summary>
    /// 出网结果：链接串以 AES-256-GCM 密文返回（不直接返回明文），并附带解密方式说明。
    /// 解密密钥取自服务端配置 Security:DeliveryKey，需调用方提前与系统共享（不随接口下发）。
    /// </summary>
    public sealed class ConnectionStringDelivery
    {
        public string ConnName { get; set; } = string.Empty;

        public string DbType { get; set; } = string.Empty;

        /// <summary>Base64(nonce[12] + ciphertext + tag[16])。</summary>
        public string ConnectionStringEncrypted { get; set; } = string.Empty;

        public string Algorithm { get; set; } = "AES-256-GCM";

        public string KeySource { get; set; } = "Security:DeliveryKey";

        public string PayloadLayout { get; set; } = "Base64(nonce[12] + ciphertext + tag[16])";

        /// <summary>解密方式说明（算法、密钥来源、载荷格式与示例文档位置）。</summary>
        public string DecryptInstructions { get; set; } = string.Empty;
    }
}
