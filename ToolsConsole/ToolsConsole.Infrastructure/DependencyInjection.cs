using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ToolsConsole.Domain.Abstractions;
using ToolsConsole.Infrastructure.Data;
using ToolsConsole.Infrastructure.Repositories;
using ToolsConsole.Infrastructure.Security;

namespace ToolsConsole.Infrastructure;

/// <summary>Infrastructure 层服务注册。</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("未配置连接串 ConnectionStrings:Default");
        }

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // 存储密钥：加解密落库的密文（默认注入，Application 层使用）
        services.AddSingleton<IConnectionStringCryptor>(
            _ => new AesGcmCryptor(configuration["Security:EncryptionKey"]));

        // 分发密钥：取串接口对返回链接串加密（与调用方共享，生产环境务必单独配置）
        services.AddKeyedSingleton<IConnectionStringCryptor>(
            CryptorKeys.Delivery,
            (_, _) => new AesGcmCryptor(configuration["Security:DeliveryKey"]));

        // 传输解密：登录/重置密码等场景解密前端提交的密码密文
        services.AddSingleton<ITransportPasswordDecryptor>(
            _ => new AesCbcPasswordDecryptor(configuration));

        return services;
    }
}
