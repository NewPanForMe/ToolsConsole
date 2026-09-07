using ToolsConsole.Application.Common;
using ToolsConsole.Application.Dtos;
using ToolsConsole.Application.Interfaces;
using ToolsConsole.Domain.Abstractions;
using ToolsConsole.Domain.Entities;

namespace ToolsConsole.Application.Services;

/// <inheritdoc cref="IDbConnService" />
public sealed class DbConnService : IDbConnService
{
    private readonly IRepository<DbConn> _dbConns;
    private readonly IConnectionStringCryptor _cryptor;

    public DbConnService(IRepository<DbConn> dbConns, IConnectionStringCryptor cryptor)
    {
        _dbConns = dbConns;
        _cryptor = cryptor;
    }

    public async Task<PagedResult<DbConnDtos.DbConnDto>> GetPageAsync(
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
            : (System.Linq.Expressions.Expression<Func<DbConn, bool>>)(c =>
                c.ConnName.Contains(kw) || c.Host.Contains(kw) || c.Database.Contains(kw));

        var total = await _dbConns.CountAsync(predicate, cancellationToken);
        var items = await _dbConns.GetListAsync(
            predicate,
            (pageIndex - 1) * pageSize,
            pageSize,
            q => q.OrderByDescending(c => c.Id),
            cancellationToken);

        return new PagedResult<DbConnDtos.DbConnDto>
        {
            List = items.Select(ToDto).ToList(),
            Total = total,
            PageIndex = pageIndex,
            PageSize = pageSize,
        };
    }

    public async Task<DbConnDtos.DbConnDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbConns.GetByIdAsync(id, cancellationToken);
        return entity is null ? null : ToDto(entity);
    }

    public async Task<DbConnDtos.DbConnDto> CreateAsync(
        DbConnDtos.DbConnCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        var connName = (request.ConnName ?? string.Empty).Trim();
        var host = (request.Host ?? string.Empty).Trim();
        ValidateBase(connName, host, request.DbType);

        if (await _dbConns.AnyAsync(c => c.ConnName == connName, cancellationToken))
        {
            throw ApiException.Conflict($"ConnName [{connName}] 已存在");
        }

        var encrypted = EncryptOrEmpty(request.Password);
        var entity = DbConn.Create(
            connName,
            request.DbType,
            host,
            request.Port,
            request.Database ?? string.Empty,
            request.Username ?? string.Empty,
            encrypted,
            NormalizeOptional(request.Options),
            NormalizeOptional(request.Remark));

        await _dbConns.AddAsync(entity, cancellationToken);
        return ToDto(entity);
    }

    public async Task<DbConnDtos.DbConnDto> UpdateAsync(
        long id,
        DbConnDtos.DbConnUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        var entity = await _dbConns.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            throw ApiException.NotFound($"连接配置不存在（id={id}）");
        }

        var connName = (request.ConnName ?? string.Empty).Trim();
        var host = (request.Host ?? string.Empty).Trim();
        ValidateBase(connName, host, request.DbType);

        if (await _dbConns.AnyAsync(c => c.Id != id && c.ConnName == connName, cancellationToken))
        {
            throw ApiException.Conflict($"ConnName [{connName}] 已存在");
        }

        // 密码语义：null / ****** → 不修改；空串 → 清除；其它 → 重新加密
        string? newEncrypted = null;
        if (request.Password is not null && request.Password != DbConnDtos.PasswordUnchanged)
        {
            newEncrypted = EncryptOrEmpty(request.Password);
        }

        entity.Update(
            connName,
            request.DbType,
            host,
            request.Port,
            request.Database ?? string.Empty,
            request.Username ?? string.Empty,
            newEncrypted,
            NormalizeOptional(request.Options),
            NormalizeOptional(request.Remark),
            request.Status);

        await _dbConns.UpdateAsync(entity, cancellationToken);
        return ToDto(entity);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        var entity = await _dbConns.GetByIdAsync(id, cancellationToken);
        if (entity is null)
        {
            throw ApiException.NotFound($"连接配置不存在（id={id}）");
        }

        await _dbConns.DeleteAsync(entity, cancellationToken);
    }

    public async Task<DbConnDtos.ResolvedConnectionString> GetConnectionStringByConnNameAsync(
        string? connName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(connName))
        {
            throw ApiException.BadRequest("connName 不能为空");
        }

        var name = connName.Trim();
        var entity = await _dbConns.FirstOrDefaultAsync(
            c => c.ConnName == name && c.Status == DbConn.StatusEnabled,
            cancellationToken);

        if (entity is null)
        {
            // 未找到与已禁用统一返回 404，避免探测“存在但被禁用”的配置
            throw ApiException.NotFound($"未找到可用的连接配置: {name}");
        }

        var password = string.IsNullOrEmpty(entity.PasswordEncrypted)
            ? string.Empty
            : _cryptor.Decrypt(entity.PasswordEncrypted);

        var connectionString = ConnectionStringBuilder.Build(
            entity.DbType,
            entity.Host,
            entity.Port,
            entity.Database,
            entity.Username,
            password,
            entity.Options);

        return new DbConnDtos.ResolvedConnectionString
        {
            ConnName = entity.ConnName,
            DbType = entity.DbType,
            ConnectionString = connectionString,
        };
    }

    private static void ValidateBase(string connName, string host, string? dbType)
    {
        if (string.IsNullOrWhiteSpace(connName))
        {
            throw ApiException.BadRequest("ConnName 不能为空");
        }

        if (string.IsNullOrWhiteSpace(host))
        {
            throw ApiException.BadRequest("Host 不能为空");
        }

        // DbType 不在支持范围内时由组装阶段报错；创建/编辑阶段仅做基础非空校验
        if (string.IsNullOrWhiteSpace(dbType))
        {
            throw ApiException.BadRequest("DbType 不能为空");
        }
    }

    private string EncryptOrEmpty(string? plainPassword)
        => string.IsNullOrEmpty(plainPassword) ? string.Empty : _cryptor.Encrypt(plainPassword);

    private static string? NormalizeOptional(string? value)
    {
        var trimmed = (value ?? string.Empty).Trim();
        return trimmed.Length == 0 ? null : trimmed;
    }

    private static DbConnDtos.DbConnDto ToDto(DbConn entity)
        => new()
        {
            Id = entity.Id,
            ConnName = entity.ConnName,
            DbType = entity.DbType,
            Host = entity.Host,
            Port = entity.Port,
            Database = entity.Database,
            Username = entity.Username,
            Options = entity.Options,
            Remark = entity.Remark,
            Status = entity.Status,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };
}
