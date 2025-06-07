using MotoHub.Application.DTOs;
using MotoHub.Domain.Common;

namespace MotoHub.Application.Interfaces.UseCases.Motorcycles;

public interface IGetMotorcycleByIdentifierUseCase
{
    Task<Result<MotorcycleDto>> ExecuteAsync(string identifier, CancellationToken cancellationToken = default);
}