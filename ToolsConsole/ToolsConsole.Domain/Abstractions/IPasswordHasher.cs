namespace ToolsConsole.Domain.Abstractions;

/// <summary>密码哈希抽象（实现不得保存或返回明文）。</summary>
public interface IPasswordHasher
{
    /// <summary>生成加盐哈希串。</summary>
    string Hash(string password);

    /// <summary>校验明文密码与哈希串是否匹配。</summary>
    bool Verify(string password, string hashedPassword);
}
