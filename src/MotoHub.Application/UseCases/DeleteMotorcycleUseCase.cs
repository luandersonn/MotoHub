using MotoHub.Application.Interfaces;
using MotoHub.Application.Interfaces.UseCases;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;

namespace MotoHub.Application.UseCases;

public class DeleteMotorcycleUseCase(IMotorcycleRepository motorcycleRepository) : IDeleteMotorcycleUseCase
{
    public async Task<Result> ExecuteAsync(string identifier, CancellationToken cancellationToken = default)
    {
        Motorcycle? motorcycle = await motorcycleRepository.GetByIdentifierAsync(identifier, cancellationToken);

        if (motorcycle is null)
        {
            return Result.Failure("Moto não encontrada", ResultErrorType.NotFound);
        }

        await motorcycleRepository.DeleteAsync(motorcycle.Id!, cancellationToken);

        return Result.Success();
    }
}
