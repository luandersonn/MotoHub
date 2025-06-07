using MotoHub.Application.DTOs;
using MotoHub.Domain.Common;

namespace MotoHub.Application.Interfaces.UseCases;

public interface IUpdateMotorcycleUseCase
{
    Task<Result<MotorcycleDto>> ExecuteAsync(string identifier, UpdateMotorcycleDto dto, CancellationToken cancellationToken = default);
}