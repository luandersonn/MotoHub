using MotoHub.Domain.Entities;
using MotoHub.Domain.Interfaces;

namespace MotoHub.Application.Interfaces;

public interface IRentRepository : IRepository<Rent>
{
    Task<Rent?> GetActiveRentByMotorcycleAsync(string motorcycleIdentifier, CancellationToken cancellationToken);
    Task<List<Rent>> GetRentsByCourierAsync(string courierIdentifier, CancellationToken cancellationToken);
    Task<List<Rent>> GetActiveRentsAsync(CancellationToken cancellationToken);
    Task<bool> IsMotorcycleCurrentlyRentedAsync(string motorcycleIdentifier, CancellationToken cancellationToken);
}