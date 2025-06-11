using MotoHub.Application.Interfaces.Repositories;
using MotoHub.Application.Interfaces.UseCases.Motorcycles;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;

namespace MotoHub.Application.UseCases.Motorcycles;

public class DeleteMotorcycleUseCase(IMotorcycleRepository motorcycleRepository) : IDeleteMotorcycleUseCase
{
    public async Task<Result> ExecuteAsync(string identifier, CancellationToken cancellationToken = default)
    {
        Motorcycle? motorcycle = await motorcycleRepository.GetByIdAsync(identifier, cancellationToken);

        if (motorcycle is null)
        {
            return Result.Failure("Moto não encontrada", ResultErrorType.NotFound);
        }

        await motorcycleRepository.DeleteAsync(motorcycle.Id, cancellationToken);

        return Result.Success();
    }
}
