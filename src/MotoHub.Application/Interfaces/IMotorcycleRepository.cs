using MotoHub.Application.DTOs;
using MotoHub.Domain.Entities;
using MotoHub.Domain.Interfaces;

namespace MotoHub.Application.Interfaces;

public interface IMotorcycleRepository : IRepository<Motorcycle>
{
    Task<Motorcycle?> GetByPlateAsync(string plate, CancellationToken cancellationToken = default);
    Task<List<Motorcycle>> SearchAsync(MotorcycleSearchParameters parameters, CancellationToken cancellationToken = default);
}