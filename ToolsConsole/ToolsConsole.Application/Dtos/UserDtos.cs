namespace ToolsConsole.Application.Dtos;

/// <summary>用户管理 DTO。</summary>
public static class UserDtos
{
    /// <summary>内置管理员账号（不允许删除/禁用）。</summary>
    public const string BuiltInAdminName = "admin";

    /// <summary>展示/编辑回显 DTO（不含任何密码信息）。</summary>
    public sealed class UserDto
    {
        public long Id { get; set; }

        public string UserName { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        /// <summary>1-启用 0-禁用。</summary>
        public int Status { get; set; } = 1;

        public DateTime? LastLoginAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>新增用户。</summary>
    public sealed class CreateUserRequest
    {
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string? DisplayName { get; set; }
    }

    /// <summary>编辑用户资料/状态。DisplayName 空 = 保持不变；Status 为 null = 保持不变。</summary>
    public sealed class UpdateUserRequest
    {
        public string? DisplayName { get; set; }

        public int? Status { get; set; }
    }

    /// <summary>重置密码。</summary>
    public sealed class ResetPasswordRequest
    {
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>当前操作者（由 Api 层从 JWT Claims 提取）。</summary>
    public sealed class OperatorInfo
    {
        public long UserId { get; set; }

        public string UserName { get; set; } = string.Empty;
    }
}
