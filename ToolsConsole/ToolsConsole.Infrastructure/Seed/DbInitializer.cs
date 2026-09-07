using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ToolsConsole.Domain.Abstractions;
using ToolsConsole.Domain.Entities;
using ToolsConsole.Infrastructure.Data;

namespace ToolsConsole.Infrastructure.Seed;

/// <summary>
/// 启动初始化：建库建表（EF EnsureCreated）+ 播种默认管理员 admin/123。
/// 说明：本 MVP 使用 EnsureCreated 自动建库建表；如需切换到 EF 迁移，
/// 请在具备网络的环境执行 `dotnet ef migrations add Init...` 并改用 MigrateAsync。
/// </summary>
public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DbInitializer");
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        try
        {
            await db.Database.EnsureCreatedAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "数据库建库/建表失败，请检查 ConnectionStrings:Default 与网络连通性");
            throw;
        }

        if (!await db.Users.AnyAsync(cancellationToken))
        {
            db.Users.Add(User.Create("admin", passwordHasher.Hash("123"), "系统管理员"));
            await db.SaveChangesAsync(cancellationToken);
            logger.LogInformation("已播种默认管理员账户 admin");
        }
    }
}
