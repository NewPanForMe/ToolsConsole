namespace ToolsConsole.Infrastructure.Configuration;

/// <summary>系统 Key 的开发默认值，仅用于首次初始化或数据库配置缺失时兜底。</summary>
public static class SystemConfigDefaults
{
    public const string JwtKey = "ToolsConsole-Jwt-Secret-Key-01dnwandl3302mklm3k43789f";

    public const string EncryptionKey = "MDEyMzQ1Njc4OWFiY2RlZjAxMjM0NTY3ODlhYmNkZWY=";

    public const string DeliveryKey = "ZmVkY2JhOTg3NjU0MzIxMGZlZGNiYTk4NzY1NDMyMTA=";

    public const string TransportKey = "MDEyMzQ1Njc4OWFiY2RlZjAxMjM0NTY3ODlhYmNkZWY=";

    public const string TransportIv = "MDEyMzQ1Njc4OWFiY2RlZg==";
}
