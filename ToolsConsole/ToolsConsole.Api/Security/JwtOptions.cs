using Microsoft.Extensions.Options;

namespace ToolsConsole.Api.Security;

/// <summary>JWT 配置（对应 appsettings JwtSettings 节）。</summary>
public sealed class JwtOptions
{
    public string Issuer { get; set; } = "ToolsConsole";

    public string Audience { get; set; } = "ToolsConsole";

    public string Key { get; set; } = string.Empty;

    public int ExpireMinutes { get; set; } = 480;
}
