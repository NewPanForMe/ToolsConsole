namespace ToolsConsole.Infrastructure.Security;

/// <summary>密钥注册名（供键控 DI 区分用途）。</summary>
public static class CryptorKeys
{
    /// <summary>存储密钥（默认注入，供读取配置密文/落库加密）。</summary>
    public const string Storage = "storage";

    /// <summary>分发密钥（供取串接口对返回链接串加密，需与调用方共享）。</summary>
    public const string Delivery = "delivery";
}
