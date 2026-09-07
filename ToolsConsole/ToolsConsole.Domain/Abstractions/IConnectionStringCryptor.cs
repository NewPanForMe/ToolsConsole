namespace ToolsConsole.Domain.Abstractions;

/// <summary>敏感字段对称加解密抽象（用于链接串中的数据库密码）。</summary>
public interface IConnectionStringCryptor
{
    /// <summary>加密明文，返回 Base64 密文（内含随机 nonce 与认证标签）。</summary>
    string Encrypt(string plainText);

    /// <summary>解密 Base64 密文，返回明文。</summary>
    string Decrypt(string cipherText);
}
