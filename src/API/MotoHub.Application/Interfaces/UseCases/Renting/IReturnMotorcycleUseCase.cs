using MotoHub.Application.DTOs;
using MotoHub.Domain.Common;

namespace MotoHub.Application.Interfaces.UseCases.Renting;

public interface IReturnMotorcycleUseCase
{
    Task<Result<CompletedRentalDto>> ExecuteAsync(ReturnMotorcycleDto dto, CancellationToken cancellationToken = default);
}