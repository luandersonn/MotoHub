using Microsoft.EntityFrameworkCore;
using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces;
using MotoHub.Domain.Entities;
using MotoHub.Infrastructure.Persistence;

namespace MotoHub.Infrastructure.Repositories;

public class MotorcycleRepository(AppDbContext context) : RepositoryBase<Motorcycle>(context), IMotorcycleRepository
{
    public async Task<Motorcycle?> GetByPlateAsync(string plate, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Plate == plate, cancellationToken);
    }

    public async Task<List<Motorcycle>> SearchAsync(MotorcycleSearchParameters queryParameters, CancellationToken cancellationToken = default)
    {
        IQueryable<Motorcycle> query = DbSet.Where(c => c.DeletedAt == null)
                                            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(queryParameters.Identifier))
        {
            query = query.Where(m => m.Identifier.Contains(queryParameters.Identifier));
        }

        if (!string.IsNullOrWhiteSpace(queryParameters.Model))
        {
            query = query.Where(m => m.Model.Contains(queryParameters.Model));
        }

        if (!string.IsNullOrWhiteSpace(queryParameters.Plate))
        {
            query = query.Where(m => m.Plate.Contains(queryParameters.Plate));
        }

        if (queryParameters.Year.HasValue)
        {
            query = query.Where(m => m.Year == queryParameters.Year.Value);
        }

        return await query.Skip(queryParameters.Offset)
                          .Take(queryParameters.Limit)
                          .ToListAsync(cancellationToken);
    }
}