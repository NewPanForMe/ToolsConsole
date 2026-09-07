using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToolsConsole.Domain.Entities;

namespace ToolsConsole.Infrastructure.Data.Configurations;

/// <summary>users 表映射（snake_case）。</summary>
public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id).HasColumnName("id").UseIdentityByDefaultColumn();
        builder.Property(u => u.UserName).HasColumnName("user_name").HasMaxLength(64).IsRequired();
        builder.Property(u => u.PasswordHash).HasColumnName("password_hash").HasColumnType("text").IsRequired();
        builder.Property(u => u.DisplayName).HasColumnName("display_name").HasMaxLength(64).IsRequired();
        builder.Property(u => u.Status).HasColumnName("status").HasDefaultValue(1);
        builder.Property(u => u.LastLoginAt).HasColumnName("last_login_at").HasColumnType("timestamp without time zone");
        builder.Property(u => u.CreatedAt).HasColumnName("created_at").HasColumnType("timestamp without time zone");
        builder.Property(u => u.UpdatedAt).HasColumnName("updated_at").HasColumnType("timestamp without time zone");

        builder.HasIndex(u => u.UserName).IsUnique();
    }
}
