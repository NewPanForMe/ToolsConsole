using ToolsConsole.Application.Dtos;

namespace ToolsConsole.Application.Interfaces;

/// <summary>用户管理服务。</summary>
public interface IUserService
{
    Task<PagedResult<UserDtos.UserDto>> GetPageAsync(
        string? keyword,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<UserDtos.UserDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>新增用户（用户名唯一；密码经哈希后存储，永不存明文）。</summary>
    Task<UserDtos.UserDto> CreateAsync(
        UserDtos.CreateUserRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>编辑资料/状态。禁止：把内置 admin 或当前登录者自己禁用。</summary>
    Task<UserDtos.UserDto> UpdateAsync(
        long id,
        UserDtos.UpdateUserRequest request,
        UserDtos.OperatorInfo operatorInfo,
        CancellationToken cancellationToken = default);

    /// <summary>重置指定用户密码。</summary>
    Task ResetPasswordAsync(
        long id,
        UserDtos.ResetPasswordRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>删除用户。禁止删除内置 admin 或当前登录者自己。</summary>
    Task DeleteAsync(long id, UserDtos.OperatorInfo operatorInfo, CancellationToken cancellationToken = default);
}
