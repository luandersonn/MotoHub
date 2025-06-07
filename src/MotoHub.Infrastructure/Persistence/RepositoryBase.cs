using Microsoft.EntityFrameworkCore;
using MotoHub.Domain.Interfaces;

namespace MotoHub.Infrastructure.Persistence;

public class RepositoryBase<T> : IRepository<T> where T : class, IEntity
{
    protected AppDbContext Context { get; }
    protected DbSet<T> DbSet { get; }

    public RepositoryBase(AppDbContext context)
    {
        Context = context;
        DbSet = Context.Set<T>();
    }

    public virtual async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public virtual async Task DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        T? entity = await DbSet.FindAsync([id], cancellationToken: cancellationToken);
        if (entity != null)
        {
            DbSet.Remove(entity);
            await Context.SaveChangesAsync(cancellationToken);
        }
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(e => e.DeletedAt == null)
                          .AsNoTracking()
                          .ToListAsync(cancellationToken: cancellationToken);
    }

    public virtual async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(e => e.DeletedAt == null)
                          .FirstOrDefaultAsync(e => e.Id == id, cancellationToken: cancellationToken);
    }

    public async Task<T?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(e => e.DeletedAt == null)
                          .AsNoTracking()
                          .FirstOrDefaultAsync(e => e.Identifier == identifier, cancellationToken: cancellationToken);
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        await Context.SaveChangesAsync(cancellationToken);
    }
}