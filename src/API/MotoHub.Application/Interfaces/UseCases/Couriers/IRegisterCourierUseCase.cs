using MotoHub.Application.DTOs;
using MotoHub.Domain.Common;

namespace MotoHub.Application.Interfaces.UseCases.Couriers;

public interface IRegisterCourierUseCase
{
    Task<Result<CourierDto>> ExecuteAsync(RegisterCourierDto dto, CancellationToken cancellationToken);
}