using MotoHub.Application.DTOs;
using MotoHub.Domain.Common;

namespace MotoHub.Application.Interfaces.UseCases.Motorcycles;

public interface ISearchMotorcyclesUseCase
{
    Task<Result<List<MotorcycleDto>>> ExecuteAsync(MotorcycleSearchParameters dto, CancellationToken cancellationToken = default);
}