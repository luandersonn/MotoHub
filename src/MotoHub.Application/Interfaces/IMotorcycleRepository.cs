using MotoHub.Application.DTOs;
using MotoHub.Domain.Entities;

namespace MotoHub.Application.Interfaces;

public interface IMotorcycleRepository : IRepository<Motorcycle>
{
    Task<Motorcycle?> GetByIdentifierAsync(string identifier, CancellationToken cancellationToken = default);
    Task<Motorcycle?> GetByPlateAsync(string plate, CancellationToken cancellationToken = default);
    Task<List<Motorcycle>> SearchAsync(MotorcycleSearchParametersDto queryParameters, CancellationToken cancellationToken = default);
}