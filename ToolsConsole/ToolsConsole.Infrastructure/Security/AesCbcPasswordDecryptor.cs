using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using ToolsConsole.Domain.Abstractions;

namespace ToolsConsole.Infrastructure.Security;

/// <summary>
/// AES-256-CBC(PKCS7) 传输解密。与前端（Web Crypto aes-256-cbc）约定的密钥/IV：
/// Security:TransportKey（32字节密钥的Base64）、Security:TransportIv（16字节IV的Base64）；
/// 未配置时使用开发默认值（与前端 dev 常量一致，生产环境必须双方单独配置并保持一致）。
/// </summary>
public sealed class AesCbcPasswordDecryptor : ITransportPasswordDecryptor
{
    // 开发默认值：key = utf8("0123456789abcdef0123456789abcdef")，iv = utf8("0123456789abcdef")
    public const string DevKeyBase64 = "MDEyMzQ1Njc4OWFiY2RlZjAxMjM0NTY3ODlhYmNkZWY=";
    public const string DevIvBase64 = "MDEyMzQ1Njc4OWFiY2RlZg==";

    private readonly byte[] _key;
    private readonly byte[] _iv;

    public AesCbcPasswordDecryptor(IConfiguration configuration)
        : this(
            configuration?["Security:TransportKey"],
            configuration?["Security:TransportIv"])
    {
    }

    public AesCbcPasswordDecryptor(string? keyBase64, string? ivBase64)
    {
        try
        {
            _key = Convert.FromBase64String(string.IsNullOrWhiteSpace(keyBase64) ? DevKeyBase64 : keyBase64);
            _iv = Convert.FromBase64String(string.IsNullOrWhiteSpace(ivBase64) ? DevIvBase64 : ivBase64);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException("Security:TransportKey/TransportIv 不是合法的 Base64", ex);
        }

        if (_key.Length != 32)
        {
            throw new InvalidOperationException("Security:TransportKey 必须是 32 字节密钥的 Base64");
        }

        if (_iv.Length != 16)
        {
            throw new InvalidOperationException("Security:TransportIv 必须是 16 字节 IV 的 Base64");
        }
    }

    public string Decrypt(string encryptedPassword)
    {
        if (string.IsNullOrWhiteSpace(encryptedPassword))
        {
            throw new CryptographicException("密码密文为空");
        }

        byte[] cipher;
        try
        {
            cipher = Convert.FromBase64String(encryptedPassword);
        }
        catch (FormatException)
        {
            throw new CryptographicException("密码密文不是合法的 Base64");
        }

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var decryptor = aes.CreateDecryptor();
        var plain = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
        return Encoding.UTF8.GetString(plain);
    }
}
