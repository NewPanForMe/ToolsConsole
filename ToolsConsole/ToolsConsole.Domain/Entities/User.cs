using ToolsConsole.Domain.Common;

namespace ToolsConsole.Domain.Entities;

/// <summary>登录用户（仅保存 PBKDF2 加盐哈希，不保存明文密码）。</summary>
public sealed class User : EntityBase
{
    public const int StatusEnabled = 1;
    public const int StatusDisabled = 0;

    private User()
    {
        // 供 EF Core 物化使用
    }

    public string UserName { get; private set; } = string.Empty;

    /// <summary>PBKDF2 哈希串（salt:hash 格式，Base64）。</summary>
    public string PasswordHash { get; private set; } = string.Empty;

    public string DisplayName { get; private set; } = string.Empty;

    /// <summary>1-启用 0-禁用。</summary>
    public int Status { get; private set; } = 1;

    public DateTime? LastLoginAt { get; private set; }

    public static User Create(string userName, string passwordHash, string displayName = "")
    {
        var name = (userName ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("用户名不能为空", nameof(userName));
        }

        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("密码哈希不能为空", nameof(passwordHash));
        }

        return new User
        {
            UserName = name,
            PasswordHash = passwordHash,
            DisplayName = string.IsNullOrWhiteSpace(displayName) ? name : displayName.Trim(),
            Status = 1,
        };
    }

    public void ChangePassword(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new ArgumentException("密码哈希不能为空", nameof(passwordHash));
        }

        PasswordHash = passwordHash;
        MarkUpdated();
    }

    public void RecordLogin()
    {
        LastLoginAt = DateTime.Now;
        MarkUpdated();
    }

    public void SetStatus(int status)
    {
        Status = status;
        MarkUpdated();
    }

    /// <summary>更新显示名（为空则保持不变），并支持同步状态。</summary>
    public void UpdateProfile(string? displayName, int? status = null)
    {
        if (!string.IsNullOrWhiteSpace(displayName))
        {
            DisplayName = displayName.Trim();
        }

        if (status is not null && status.Value != Status)
        {
            Status = status.Value;
        }

        MarkUpdated();
    }
}
