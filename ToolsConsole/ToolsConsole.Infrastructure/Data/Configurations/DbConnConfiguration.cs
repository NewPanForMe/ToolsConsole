using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToolsConsole.Domain.Entities;

namespace ToolsConsole.Infrastructure.Data.Configurations;

/// <summary>db_conns 表映射（snake_case；密码只存密文列 password_enc）。</summary>
public sealed class DbConnConfiguration : IEntityTypeConfiguration<DbConn>
{
    public void Configure(EntityTypeBuilder<DbConn> builder)
    {
        builder.ToTable("db_conns");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id).HasColumnName("id").UseIdentityByDefaultColumn();
        builder.Property(c => c.ConnName).HasColumnName("conn_name").HasMaxLength(100).IsRequired();
        builder.Property(c => c.DbType).HasColumnName("db_type").HasMaxLength(20).IsRequired().HasDefaultValue("PostgreSQL");
        builder.Property(c => c.Host).HasColumnName("host").HasMaxLength(255).IsRequired();
        builder.Property(c => c.Port).HasColumnName("port").HasDefaultValue(DbConn.DefaultPort);
        builder.Property(c => c.Database).HasColumnName("database").HasMaxLength(255).IsRequired().HasDefaultValue(string.Empty);
        builder.Property(c => c.Username).HasColumnName("username").HasMaxLength(255).IsRequired().HasDefaultValue(string.Empty);
        builder.Property(c => c.PasswordEncrypted).HasColumnName("password_enc").HasColumnType("text").IsRequired().HasDefaultValue(string.Empty);
        builder.Property(c => c.Options).HasColumnName("options").HasColumnType("text");
        builder.Property(c => c.Remark).HasColumnName("remark").HasMaxLength(500);
        builder.Property(c => c.Status).HasColumnName("status").HasDefaultValue(DbConn.StatusEnabled);
        builder.Property(c => c.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");

        builder.HasIndex(c => c.ConnName).IsUnique();
    }
}
