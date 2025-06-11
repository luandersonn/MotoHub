using MotoHub.Application.DTOs;
using MotoHub.Domain.Common;

namespace MotoHub.Application.Interfaces.UseCases.Couriers;

public interface IUpdateCourierUseCase
{
    Task<Result<CourierDto>> ExecuteAsync(string identifier, UpdateCourierDto dto, CancellationToken cancellationToken = default);
}