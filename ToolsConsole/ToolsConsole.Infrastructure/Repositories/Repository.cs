using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ToolsConsole.Domain.Abstractions;
using ToolsConsole.Infrastructure.Data;

namespace ToolsConsole.Infrastructure.Repositories;

/// <summary>基于 EF Core 的通用仓储。</summary>
public sealed class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly AppDbContext _db;
    private readonly DbSet<TEntity> _set;

    public Repository(AppDbContext db)
    {
        _db = db;
        _set = db.Set<TEntity>();
    }

    public async Task<TEntity?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        => await _set.FindAsync(new object[] { id }, cancellationToken);

    public async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await _set.FirstOrDefaultAsync(predicate, cancellationToken);

    public Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => _set.AnyAsync(predicate, cancellationToken);

    public Task<long> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
        => predicate is null ? _set.LongCountAsync(cancellationToken) : _set.LongCountAsync(predicate, cancellationToken);

    public Task<List<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        int skip = 0,
        int take = 0,
        CancellationToken cancellationToken = default)
        => GetListAsync(predicate, skip, take, null, cancellationToken);

    public async Task<List<TEntity>> GetListAsync(
        Expression<Func<TEntity, bool>>? predicate,
        int skip,
        int take,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy,
        CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = predicate is null ? _set : _set.Where(predicate);
        if (orderBy is not null)
        {
            query = orderBy(query);
        }

        if (take > 0)
        {
            query = query.Skip(skip).Take(take);
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _set.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _set.Update(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        _set.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
