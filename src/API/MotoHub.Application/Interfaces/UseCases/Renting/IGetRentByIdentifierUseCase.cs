using MotoHub.Application.DTOs;
using MotoHub.Domain.Common;

namespace MotoHub.Application.Interfaces.UseCases.Renting;

public interface IGetRentByIdentifierUseCase
{
    Task<Result<RentDto>> ExecuteAsync(string identifier, CancellationToken cancellationToken = default);
}