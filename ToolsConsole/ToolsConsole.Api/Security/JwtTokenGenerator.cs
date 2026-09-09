using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ToolsConsole.Application.Dtos;
using ToolsConsole.Domain.Abstractions;
using ToolsConsole.Infrastructure.Configuration;

namespace ToolsConsole.Api.Security;

/// <summary>JWT 签发。</summary>
public sealed class JwtTokenGenerator
{
    private readonly JwtOptions _options;
    private readonly ISystemConfigProvider _systemConfigProvider;

    public JwtTokenGenerator(IOptions<JwtOptions> options, ISystemConfigProvider systemConfigProvider)
    {
        _options = options.Value;
        _systemConfigProvider = systemConfigProvider;
    }

    /// <summary>Token 有效期（秒）。</summary>
    public int ExpireSeconds => (int)TimeSpan.FromMinutes(_options.ExpireMinutes).TotalSeconds;

    public string CreateToken(LoginResult login)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, login.UserId.ToString()),
            new Claim(ClaimTypes.Name, login.UserName),
            new Claim("displayName", login.DisplayName),
        };

        var keyText = _systemConfigProvider.GetValue("JwtSettings:Key")
            ?? _options.Key
            ?? SystemConfigDefaults.JwtKey;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyText));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var now = DateTime.UtcNow;

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_options.ExpireMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
