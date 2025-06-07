using MotoHub.Application.DTOs;
using MotoHub.Domain.Common;

namespace MotoHub.Application.Interfaces.UseCases.Renting;

public interface IRentMotorcycleUseCase
{
    Task<Result<RentDto>> ExecuteAsync(RentMotorcycleDto dto, CancellationToken cancellationToken = default);
}