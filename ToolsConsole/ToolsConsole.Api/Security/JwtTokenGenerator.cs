using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ToolsConsole.Application.Dtos;

namespace ToolsConsole.Api.Security;

/// <summary>JWT 签发。</summary>
public sealed class JwtTokenGenerator
{
    private readonly JwtOptions _options;

    public JwtTokenGenerator(IOptions<JwtOptions> options)
    {
        _options = options.Value;
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

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
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
