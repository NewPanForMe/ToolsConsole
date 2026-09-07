using System.Security.Cryptography;
using System.Text;
using ToolsConsole.Domain.Abstractions;

namespace ToolsConsole.Infrastructure.Security;

/// <summary>
/// AES-256-GCM 加解密。密钥来自配置 Security:EncryptionKey（32 字节 Base64），
/// 未配置时使用开发默认密钥（仅限本地开发，正式环境必须配置）。
/// 密文格式：Base64( nonce[12] + ciphertext + tag[16] )。
/// </summary>
public sealed class AesGcmCryptor : IConnectionStringCryptor
{
    private const int NonceSize = 12;
    private const int TagSize = 16;

    // "0123456789abcdef0123456789abcdef" 的 Base64 —— 仅开发默认值
    private const string DefaultDevKeyBase64 = "MDEyMzQ1Njc4OWFiY2RlZjAxMjM0NTY3ODlhYmNkZWY=";

    private readonly byte[] _key;

    public AesGcmCryptor(string? base64Key = null)
    {
        var keyBase64 = string.IsNullOrWhiteSpace(base64Key) ? DefaultDevKeyBase64 : base64Key;
        try
        {
            _key = Convert.FromBase64String(keyBase64);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException("Security:EncryptionKey 不是合法的 Base64 字符串", ex);
        }

        if (_key.Length != 32)
        {
            throw new InvalidOperationException("Security:EncryptionKey 必须是 32 字节（256 位）密钥的 Base64 表示");
        }
    }

    public string Encrypt(string plainText)
    {
        if (plainText is null)
        {
            throw new ArgumentNullException(nameof(plainText));
        }

        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var nonce = RandomNumberGenerator.GetBytes(NonceSize);
        var cipher = new byte[plainBytes.Length];
        var tag = new byte[TagSize];

        using var aes = new AesGcm(_key, TagSize);
        aes.Encrypt(nonce, plainBytes, cipher, tag);

        var payload = new byte[NonceSize + cipher.Length + TagSize];
        Buffer.BlockCopy(nonce, 0, payload, 0, NonceSize);
        Buffer.BlockCopy(cipher, 0, payload, NonceSize, cipher.Length);
        Buffer.BlockCopy(tag, 0, payload, NonceSize + cipher.Length, TagSize);
        return Convert.ToBase64String(payload);
    }

    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrWhiteSpace(cipherText))
        {
            return string.Empty;
        }

        byte[] payload;
        try
        {
            payload = Convert.FromBase64String(cipherText);
        }
        catch (FormatException)
        {
            throw new CryptographicException("密文不是合法的 Base64 字符串");
        }

        if (payload.Length < NonceSize + TagSize)
        {
            throw new CryptographicException("密文长度不合法");
        }

        var nonce = new byte[NonceSize];
        var cipher = new byte[payload.Length - NonceSize - TagSize];
        var tag = new byte[TagSize];
        Buffer.BlockCopy(payload, 0, nonce, 0, NonceSize);
        Buffer.BlockCopy(payload, NonceSize, cipher, 0, cipher.Length);
        Buffer.BlockCopy(payload, NonceSize + cipher.Length, tag, 0, TagSize);

        var plain = new byte[cipher.Length];
        using var aes = new AesGcm(_key, TagSize);
        aes.Decrypt(nonce, cipher, tag, plain);
        return Encoding.UTF8.GetString(plain);
    }
}
