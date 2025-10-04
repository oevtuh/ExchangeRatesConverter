using Microsoft.EntityFrameworkCore;
using RatesConverterAPI.Domain.Data;
using RatesConverterAPI.Domain.Interfaces;

namespace RatesConverterAPI.Domain.Repositories;

/// <summary>
/// Minimal EF Core repository implementation for any entity type.
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public class EfRepository<T> : IRepository<T> where T : class
{
    private readonly RatesConverterDbContext _db;
    private readonly DbSet<T> _set;

    public EfRepository(RatesConverterDbContext db)
    {
        _db = db;
        _set = _db.Set<T>();
    }

    public IQueryable<T> Query(bool asNoTracking = false)
        => asNoTracking ? _set.AsNoTracking() : _set.AsQueryable();

    public Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        => _set.FindAsync([id], cancellationToken).AsTask();

    public Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => _set.AddAsync(entity, cancellationToken).AsTask();

    public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        => _set.AddRangeAsync(entities, cancellationToken);

    public void Update(T entity) => _set.Update(entity);

    public void UpdateRange(IEnumerable<T> entities) => _set.UpdateRange(entities);

    public void Remove(T entity) => _set.Remove(entity);

    public void RemoveRange(IEnumerable<T> entities) => _set.RemoveRange(entities);

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _db.SaveChangesAsync(cancellationToken);
}
