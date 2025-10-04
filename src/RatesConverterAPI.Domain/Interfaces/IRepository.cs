using System.Linq.Expressions;

namespace RatesConverterAPI.Domain.Interfaces;

/// <summary>
/// Minimal generic repository abstraction for EF Core-backed entities.
/// Keep it simple: query, basic CRUD, and SaveChanges.
/// </summary>
/// <typeparam name="T">Entity type</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Returns an <see cref="IQueryable{T}"/> for further composition.
    /// </summary>
    /// <param name="asNoTracking">If true, disables change tracking.</param>
    IQueryable<T> Query(bool asNoTracking = false);

    /// <summary>
    /// Finds an entity by its primary key value.
    /// </summary>
    /// <param name="id">Primary key value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<T?> GetByIdAsync(object id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new entity.
    /// </summary>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds multiple entities in a batch.
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Marks an entity as modified.
    /// </summary>
    void Update(T entity);

    /// <summary>
    /// Marks multiple entities as modified.
    /// </summary>
    void UpdateRange(IEnumerable<T> entities);

    /// <summary>
    /// Removes an entity.
    /// </summary>
    void Remove(T entity);

    /// <summary>
    /// Removes multiple entities.
    /// </summary>
    void RemoveRange(IEnumerable<T> entities);

    /// <summary>
    /// Persists pending changes to the database.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
