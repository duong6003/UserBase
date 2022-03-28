using Core.Bases;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repositories;

public interface IRepositoryBase<T> where T : class
{
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);

    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;

    IQueryable<T> FindAll(bool isAsNoTracking = default);

    IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool isAsNoTracking = default);
    Task<bool> IsAnyValue(Expression<Func<T, bool>> expression);
}

public class RepositoryBase<T> : IRepositoryBase<T> where T : class
{
    private readonly ApplicationDbContext DbContext;

    public RepositoryBase(ApplicationDbContext dbContext) => DbContext = dbContext;
    public async Task<bool> IsAnyValue(Expression<Func<T, bool>> expression)
    {
        return await DbContext.Set<T>().AnyAsync(expression);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbContext.Set<T>().AddAsync(entity, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await DbContext.Set<T>().AddRangeAsync(entities, cancellationToken);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().Update(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().UpdateRange(entities);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().Remove(entity);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        DbContext.Set<T>().RemoveRange(entities);
        await DbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<T?> GetByIdAsync<TId>(TId? id, CancellationToken cancellationToken = default) where TId : notnull
    {
        return await DbContext.Set<T>().FindAsync(new object?[] { id }, cancellationToken: cancellationToken);
    }

    public IQueryable<T> FindAll(bool isAsNoTracking = default)
    {
        return isAsNoTracking ? DbContext.Set<T>().AsNoTracking() : DbContext.Set<T>();
    }

    public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool isAsNoTracking = default)
    {
        return isAsNoTracking ? DbContext.Set<T>().AsNoTracking().Where(expression) : DbContext.Set<T>().Where(expression);
    }
}
