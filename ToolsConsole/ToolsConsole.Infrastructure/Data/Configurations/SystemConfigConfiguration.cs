using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToolsConsole.Domain.Entities;

namespace ToolsConsole.Infrastructure.Data.Configurations;

/// <summary>system_configs 表映射，保存系统运行 Key。</summary>
public sealed class SystemConfigConfiguration : IEntityTypeConfiguration<SystemConfig>
{
    public void Configure(EntityTypeBuilder<SystemConfig> builder)
    {
        builder.ToTable("system_configs");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).HasColumnName("id").UseIdentityByDefaultColumn();
        builder.Property(c => c.ConfigKey).HasColumnName("config_key").HasMaxLength(120).IsRequired();
        builder.Property(c => c.ConfigValue).HasColumnName("config_value").HasColumnType("text").IsRequired();
        builder.Property(c => c.Remark).HasColumnName("remark").HasMaxLength(500);
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");

        builder.HasIndex(c => c.ConfigKey).IsUnique();
    }
}
