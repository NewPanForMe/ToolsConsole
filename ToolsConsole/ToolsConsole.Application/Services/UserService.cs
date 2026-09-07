using System.Text.RegularExpressions;
using ToolsConsole.Application.Common;
using ToolsConsole.Application.Dtos;
using ToolsConsole.Application.Interfaces;
using ToolsConsole.Domain.Abstractions;
using ToolsConsole.Domain.Entities;

namespace ToolsConsole.Application.Services;

/// <inheritdoc cref="IUserService" />
public sealed partial class UserService : IUserService
{
    private const int PasswordMinLength = 3;
    private const int PasswordMaxLength = 64;

    private readonly IRepository<User> _users;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IRepository<User> users, IPasswordHasher passwordHasher)
    {
        _users = users;
        _passwordHasher = passwordHasher;
    }

    [GeneratedRegex("^[A-Za-z0-9_.-]{2,64}$")]
    private static partial Regex UserNameRegex();

    public async Task<PagedResult<UserDtos.UserDto>> GetPageAsync(
        string? keyword,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        pageIndex = Math.Max(1, pageIndex);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var kw = (keyword ?? string.Empty).Trim();
        var predicate = kw.Length == 0
            ? null
            : (System.Linq.Expressions.Expression<Func<User, bool>>)(u =>
                u.UserName.Contains(kw) || u.DisplayName.Contains(kw));

        var total = await _users.CountAsync(predicate, cancellationToken);
        var items = await _users.GetListAsync(
            predicate,
            (pageIndex - 1) * pageSize,
            pageSize,
            q => q.OrderByDescending(u => u.Id),
            cancellationToken);

        return new PagedResult<UserDtos.UserDto>
        {
            List = items.Select(ToDto).ToList(),
            Total = total,
            PageIndex = pageIndex,
            PageSize = pageSize,
        };
    }

    public async Task<UserDtos.UserDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(id, cancellationToken);
        return user is null ? null : ToDto(user);
    }

    public async Task<UserDtos.UserDto> CreateAsync(
        UserDtos.CreateUserRequest request,
        CancellationToken cancellationToken = default)
    {
        var userName = (request.UserName ?? string.Empty).Trim();
        ValidateUserName(userName);
        ValidatePassword(request.Password, requireOnCreate: true);

        if (await _users.AnyAsync(u => u.UserName == userName, cancellationToken))
        {
            throw ApiException.Conflict($"用户名 [{userName}] 已存在");
        }

        var entity = User.Create(userName, _passwordHasher.Hash(request.Password), request.DisplayName ?? string.Empty);
        await _users.AddAsync(entity, cancellationToken);
        return ToDto(entity);
    }

    public async Task<UserDtos.UserDto> UpdateAsync(
        long id,
        UserDtos.UpdateUserRequest request,
        UserDtos.OperatorInfo operatorInfo,
        CancellationToken cancellationToken = default)
    {
        var user = await RequireUserAsync(id, cancellationToken);

        var status = request.Status;
        if (status is not null && status.Value != User.StatusEnabled && status.Value != User.StatusDisabled)
        {
            throw ApiException.BadRequest("状态取值不合法（0-禁用 1-启用）");
        }

        if (status == User.StatusDisabled)
        {
            if (string.Equals(user.UserName, UserDtos.BuiltInAdminName, StringComparison.OrdinalIgnoreCase))
            {
                throw ApiException.BadRequest($"内置管理员 [{UserDtos.BuiltInAdminName}] 不允许被禁用");
            }

            if (user.Id == operatorInfo.UserId)
            {
                throw ApiException.BadRequest("不能禁用当前登录的账号");
            }
        }

        user.UpdateProfile(request.DisplayName, request.Status);
        await _users.UpdateAsync(user, cancellationToken);
        return ToDto(user);
    }

    public async Task ResetPasswordAsync(
        long id,
        UserDtos.ResetPasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        var user = await RequireUserAsync(id, cancellationToken);
        ValidatePassword(request.Password, requireOnCreate: true);
        user.ChangePassword(_passwordHasher.Hash(request.Password));
        await _users.UpdateAsync(user, cancellationToken);
    }

    public async Task DeleteAsync(
        long id,
        UserDtos.OperatorInfo operatorInfo,
        CancellationToken cancellationToken = default)
    {
        var user = await RequireUserAsync(id, cancellationToken);

        if (string.Equals(user.UserName, UserDtos.BuiltInAdminName, StringComparison.OrdinalIgnoreCase))
        {
            throw ApiException.BadRequest($"内置管理员 [{UserDtos.BuiltInAdminName}] 不允许删除");
        }

        if (user.Id == operatorInfo.UserId)
        {
            throw ApiException.BadRequest("不能删除当前登录的账号");
        }

        await _users.DeleteAsync(user, cancellationToken);
    }

    private async Task<User> RequireUserAsync(long id, CancellationToken cancellationToken)
    {
        var user = await _users.GetByIdAsync(id, cancellationToken);
        return user ?? throw ApiException.NotFound($"用户不存在（id={id}）");
    }

    private static void ValidateUserName(string userName)
    {
        if (!UserNameRegex().IsMatch(userName))
        {
            throw ApiException.BadRequest("用户名仅支持 2-64 位字母、数字、下划线、中划线或点");
        }
    }

    private static void ValidatePassword(string? password, bool requireOnCreate)
    {
        if (string.IsNullOrEmpty(password) && requireOnCreate)
        {
            throw ApiException.BadRequest("密码不能为空");
        }

        if (!string.IsNullOrEmpty(password)
            && (password.Length < PasswordMinLength || password.Length > PasswordMaxLength))
        {
            throw ApiException.BadRequest($"密码长度需为 {PasswordMinLength}-{PasswordMaxLength} 位");
        }
    }

    private static UserDtos.UserDto ToDto(User user)
        => new()
        {
            Id = user.Id,
            UserName = user.UserName,
            DisplayName = user.DisplayName,
            Status = user.Status,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
        };
}
