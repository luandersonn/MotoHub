using MotoHub.Application.DTOs;
using MotoHub.Domain.Common;

namespace MotoHub.Application.Interfaces.UseCases.Motorcycles;

public interface IRegisterMotorcycleUseCase
{
    Task<Result<MotorcycleDto>> ExecuteAsync(RegisterMotorcycleDto dto, CancellationToken cancellationToken = default);
}