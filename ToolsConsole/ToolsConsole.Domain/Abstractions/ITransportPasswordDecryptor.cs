namespace ToolsConsole.Domain.Abstractions;

/// <summary>传输层密码解密抽象：前端提交的密码为密文，由本接口解密为明文后再进入业务层（哈希/校验）。</summary>
public interface ITransportPasswordDecryptor
{
    /// <summary>解密 Base64 密文为明文；解密失败应抛出异常。</summary>
    string Decrypt(string encryptedPassword);
}
