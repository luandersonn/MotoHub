using MotoHub.Application.DTOs;
using MotoHub.Application.Events;
using MotoHub.Application.Interfaces.Messaging;
using MotoHub.Application.Interfaces.Repositories;
using MotoHub.Application.Interfaces.UseCases.Motorcycles;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;

namespace MotoHub.Application.UseCases.Motorcycles;

public class RegisterMotorcycleUseCase(IMotorcycleRepository motorcycleRepository, IMotorcycleEventPublisher eventPublisher) : IRegisterMotorcycleUseCase
{
    public async Task<Result<MotorcycleDto>> ExecuteAsync(RegisterMotorcycleDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Plate))
        {
            return Result<MotorcycleDto>.Failure("Placa inválida", ResultErrorType.ValidationError);
        }

        if (dto.Year <= 1900)
        {
            return Result<MotorcycleDto>.Failure("O ano deve ser maior que 1900", ResultErrorType.ValidationError);
        }

        Motorcycle? motorcycle = await motorcycleRepository.GetByPlateAsync(dto.Plate, cancellationToken);

        if (motorcycle is not null)
        {
            return Result<MotorcycleDto>.Failure("Já existe uma moto com esta placa registrado no sistema", ResultErrorType.BusinessError);
        }

        motorcycle = await motorcycleRepository.GetByIdAsync(dto.Identifier, cancellationToken);

        if (motorcycle is not null)
        {
            return Result<MotorcycleDto>.Failure("Já existe uma moto com este identificador no sistema", ResultErrorType.BusinessError);
        }

        motorcycle = new()
        {
            Id = dto.Identifier,
            Plate = dto.Plate,
            Year = dto.Year,
            Model = dto.Model,
        };

        await motorcycleRepository.AddAsync(motorcycle, cancellationToken);

        await eventPublisher.PublishMotorcycleRegisteredAsync(new MotorcycleRegisteredEvent(motorcycle.Id,
                                                                                            motorcycle.Plate,
                                                                                            motorcycle.Year,
                                                                                            motorcycle.Model), cancellationToken);

        MotorcycleDto resultDto = new()
        {
            Identifier = motorcycle.Id,
            Plate = motorcycle.Plate,
            Year = motorcycle.Year,
            Model = motorcycle.Model
        };

        return Result<MotorcycleDto>.Success(resultDto);
    }
}