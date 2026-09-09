using System.Text;

namespace ToolsConsole.Infrastructure.Configuration;

/// <summary>项目自身数据库连接串配置支持三次 Base64 存放。</summary>
public static class ConnectionStringDecoder
{
    public static string DecodeTripleBase64IfNeeded(string connectionString)
    {
        var value = connectionString?.Trim() ?? string.Empty;
        if (value.Length == 0)
        {
            return value;
        }

        try
        {
            for (var i = 0; i < 3; i += 1)
            {
                value = Encoding.UTF8.GetString(Convert.FromBase64String(value));
            }

            return value;
        }
        catch (FormatException)
        {
            return connectionString ?? string.Empty;
        }
    }
}
