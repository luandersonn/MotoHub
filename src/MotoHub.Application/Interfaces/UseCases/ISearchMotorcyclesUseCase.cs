using MotoHub.Application.DTOs;
using MotoHub.Domain.Common;

namespace MotoHub.Application.Interfaces.UseCases;

public interface ISearchMotorcyclesUseCase
{
    Task<Result<List<MotorcycleDto>>> ExecuteAsync(MotorcycleSearchParametersDTO dto, CancellationToken cancellationToken = default);
}