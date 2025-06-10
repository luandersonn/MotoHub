using Microsoft.EntityFrameworkCore;
using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces;
using MotoHub.Domain.Entities;
using MotoHub.Domain.ValueObjects;

namespace MotoHub.Infrastructure.Persistence;

public class UserRepository(AppDbContext context) : RepositoryBase<User>(context), IUserRepository
{
    public async Task<List<User>> GetUsersByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
    {
        return await DbSet.Where(u => u.DeletedAt == null)
                          .Where(u => u.Role == role)
                          .AsNoTracking()
                          .ToListAsync(cancellationToken);
    }

    public Task<List<User>> SearchAsync(UserSearchParameters parameters, CancellationToken cancellationToken = default)
    {
        IQueryable<User> query = DbSet.Where(u => u.DeletedAt == null)
                                      .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(parameters.Name))
        {
            query = query.Where(u => u.Name.Contains(parameters.Name));
        }

        if (!string.IsNullOrWhiteSpace(parameters.TaxNumber))
        {
            query = query.Where(u => u.TaxNumber.Contains(parameters.TaxNumber));
        }

        if (!string.IsNullOrWhiteSpace(parameters.DriverLicenseNumber))
        {
            query = query.Where(u => u.DriverLicenseNumber.Contains(parameters.DriverLicenseNumber));
        }

        if (parameters.Role.HasValue)
        {
            query = query.Where(u => u.Role == parameters.Role.Value);
        }

        return query.Skip(parameters.Offset)
                    .Take(parameters.Limit)
                    .ToListAsync(cancellationToken);
    }

    public Task<User?> GetUserByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default)
    {
        return DbSet.AsNoTracking()
                    .Where(u => u.DeletedAt == null)
                    .FirstOrDefaultAsync(u => u.DriverLicenseNumber == licenseNumber, cancellationToken);
    }

    public Task<User?> GetUserByTaxNumberAsync(string taxNumber, CancellationToken cancellationToken)
    {
        return DbSet.AsNoTracking()
                .Where(u => u.DeletedAt == null)
                .FirstOrDefaultAsync(u => u.TaxNumber == taxNumber, cancellationToken);
    }
}
