using ToolsConsole.Application.Common;
using ToolsConsole.Application.Dtos;
using ToolsConsole.Application.Interfaces;
using ToolsConsole.Domain.Abstractions;
using ToolsConsole.Domain.Entities;

namespace ToolsConsole.Application.Services;

/// <inheritdoc cref="IAuthService" />
public sealed class AuthService : IAuthService
{
    private readonly IRepository<User> _users;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IRepository<User> users, IPasswordHasher passwordHasher)
    {
        _users = users;
        _passwordHasher = passwordHasher;
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var userName = (request.UserName ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(userName))
        {
            throw ApiException.BadRequest("用户名不能为空");
        }

        if (string.IsNullOrEmpty(request.Password))
        {
            throw ApiException.BadRequest("密码不能为空");
        }

        var user = await _users.FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            throw ApiException.Unauthorized("用户名或密码错误");
        }

        if (user.Status != 1)
        {
            throw ApiException.Unauthorized("账号已被禁用");
        }

        user.RecordLogin();
        await _users.UpdateAsync(user, cancellationToken);

        return new LoginResult
        {
            UserId = user.Id,
            UserName = user.UserName,
            DisplayName = user.DisplayName,
        };
    }
}
