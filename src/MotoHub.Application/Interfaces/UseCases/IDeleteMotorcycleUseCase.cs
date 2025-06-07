using MotoHub.Domain.Common;

namespace MotoHub.Application.Interfaces.UseCases;

public interface IDeleteMotorcycleUseCase
{
    Task<Result> ExecuteAsync(string identifier, CancellationToken cancellationToken = default);
}