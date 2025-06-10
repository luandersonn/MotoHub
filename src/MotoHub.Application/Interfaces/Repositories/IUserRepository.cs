using MotoHub.Application.DTOs;
using MotoHub.Domain.Entities;
using MotoHub.Domain.Interfaces;
using MotoHub.Domain.ValueObjects;

namespace MotoHub.Application.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<List<User>> GetUsersByRoleAsync(UserRole role, CancellationToken cancellationToken = default);
    Task<List<User>> SearchAsync(UserSearchParameters parameters, CancellationToken cancellationToken = default);
    Task<User?> GetUserByLicenseNumberAsync(string licenseNumber, CancellationToken cancellationToken = default);
    Task<User?> GetUserByTaxNumberAsync(string taxNumber, CancellationToken cancellationToken);
}