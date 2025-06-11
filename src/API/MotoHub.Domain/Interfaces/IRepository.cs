namespace MotoHub.Domain.Interfaces;

public interface IRepository<T> where T : IEntity
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
    Task<T?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(long id, CancellationToken cancellationToken = default);
}