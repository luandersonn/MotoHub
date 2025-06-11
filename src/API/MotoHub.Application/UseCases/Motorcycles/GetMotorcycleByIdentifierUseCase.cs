using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces.Repositories;
using MotoHub.Application.Interfaces.UseCases.Motorcycles;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;

namespace MotoHub.Application.UseCases.Motorcycles;

public class GetMotorcycleByIdentifierUseCase(IMotorcycleRepository motorcycleRepository) : IGetMotorcycleByIdentifierUseCase
{
    public async Task<Result<MotorcycleDto>> ExecuteAsync(string identifier, CancellationToken cancellationToken = default)
    {
        Motorcycle? motorcycle = await motorcycleRepository.GetByIdAsync(identifier, cancellationToken);

        if (motorcycle is null)
        {
            return Result<MotorcycleDto>.Failure("Moto não encontrada", ResultErrorType.NotFound);
        }

        MotorcycleDto dto = new()
        {
            Identifier = motorcycle.Id,
            Plate = motorcycle.Plate,
            Year = motorcycle.Year,
            Model = motorcycle.Model
        };
        return Result<MotorcycleDto>.Success(dto);
    }
}