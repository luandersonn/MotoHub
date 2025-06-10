using Microsoft.EntityFrameworkCore;
using MotoHub.Application.Interfaces;
using MotoHub.Domain.Entities;
using MotoHub.Infrastructure.Persistence;

namespace MotoHub.Infrastructure.Repositories;

public class RentRepository(AppDbContext context) : RepositoryBase<Rent>(context), IRentRepository
{
    public Task<Rent?> GetActiveRentByMotorcycleAsync(string motorcycleIdentifier, CancellationToken cancellationToken)
    {
        return DbSet.AsNoTracking()
                    .Where(e => e.DeletedAt == null)
                    .Where(e => e.MotorcycleIdentifier == motorcycleIdentifier)
                    .Where(e => e.Status == RentStatus.Active)
                    .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<List<Rent>> GetActiveRentsAsync(CancellationToken cancellationToken)
    {
        return DbSet.AsNoTracking()
                    .Where(e => e.DeletedAt == null)
                    .Where(e => e.Status == RentStatus.Active)
                    .ToListAsync(cancellationToken);
    }

    public Task<List<Rent>> GetRentsByCourierAsync(string courierIdentifier, CancellationToken cancellationToken)
    {
        return DbSet.AsNoTracking()
                    .Where(e => e.DeletedAt == null)
                    .Where(e => e.CourierIdentifier == courierIdentifier)
                    .ToListAsync(cancellationToken);
    }

    public Task<bool> IsMotorcycleCurrentlyRentedAsync(string motorcycleIdentifier, CancellationToken cancellationToken)
    {
        return DbSet.AsNoTracking()
                    .Where(e => e.DeletedAt == null)
                    .Where(e => e.MotorcycleIdentifier == motorcycleIdentifier)
                    .Where(e => e.Status == RentStatus.Active)
                    .AnyAsync(cancellationToken);
    }
}