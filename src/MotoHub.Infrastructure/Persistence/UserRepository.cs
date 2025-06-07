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
        return await DbSet.Where(c => c.DeletedAt == null)
                          .Where(u => u.Role == role)
                          .AsNoTracking()
                          .ToListAsync(cancellationToken);
    }

    public Task<List<User>> SearchAsync(UserSearchParameters parameters, CancellationToken cancellationToken = default)
    {
        IQueryable<User> query = DbSet.Where(c => c.DeletedAt == null)
                                      .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(parameters.Name))
        {
            query = query.Where(u => u.Name.Contains(parameters.Name));
        }

        if (parameters.Role.HasValue)
        {
            query = query.Where(u => u.Role == parameters.Role.Value);
        }

        return query.Skip(parameters.Offset)
                    .Take(parameters.Limit)
                    .ToListAsync(cancellationToken);
    }
}
