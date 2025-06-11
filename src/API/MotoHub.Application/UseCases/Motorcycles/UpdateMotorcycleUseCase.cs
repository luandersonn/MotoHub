using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces.Repositories;
using MotoHub.Application.Interfaces.UseCases.Motorcycles;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;

namespace MotoHub.Application.UseCases.Motorcycles;

public class UpdateMotorcycleUseCase(IMotorcycleRepository motorcycleRepository) : IUpdateMotorcycleUseCase
{
    public async Task<Result<MotorcycleDto>> ExecuteAsync(string identifier, UpdateMotorcycleDto dto, CancellationToken cancellationToken = default)
    {
        Motorcycle? existingMotorcycle = await motorcycleRepository.GetByIdAsync(identifier, cancellationToken);

        if (existingMotorcycle is null)
        {
            return Result<MotorcycleDto>.Failure("Moto não encontrada", ResultErrorType.NotFound);
        }

        if (!string.IsNullOrWhiteSpace(dto.Plate)) // Atualizar placa
        {
            if (await IsPlateInUseAsync(dto.Plate, identifier, cancellationToken))
            {
                return Result<MotorcycleDto>.Failure("Já existe uma moto com esta placa registrada no sistema", ResultErrorType.BusinessError);
            }

            existingMotorcycle.Plate = dto.Plate;
        }

        if (dto.Year > 1900) // Atualizar ano
        {
            existingMotorcycle.Year = dto.Year.Value;
        }

        if (!string.IsNullOrWhiteSpace(dto.Model)) // Atualizar modelo
        {
            existingMotorcycle.Model = dto.Model;
        }

        await motorcycleRepository.UpdateAsync(existingMotorcycle, cancellationToken);

        MotorcycleDto resultDto = new()
        {
            Identifier = existingMotorcycle.Id,
            Plate = existingMotorcycle.Plate,
            Year = existingMotorcycle.Year,
            Model = existingMotorcycle.Model
        };

        return Result<MotorcycleDto>.Success(resultDto);
    }

    private async Task<bool> IsPlateInUseAsync(string plate, string identifier, CancellationToken cancellationToken)
    {
        Motorcycle? motorcycleWithSamePlate = await motorcycleRepository.GetByPlateAsync(plate, cancellationToken);
        return motorcycleWithSamePlate is not null && motorcycleWithSamePlate.Id != identifier;
    }
}
