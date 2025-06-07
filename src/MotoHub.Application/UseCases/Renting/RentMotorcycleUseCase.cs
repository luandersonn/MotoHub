using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces;
using MotoHub.Application.Interfaces.UseCases.Renting;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;

namespace MotoHub.Application.UseCases.Renting;

public class RentMotorcycleUseCase(IRentRepository rentRepository, IMotorcycleRepository motorcycleRepository, IUserRepository userRepository) : IRentMotorcycleUseCase
{
    public async Task<Result<RentDto>> ExecuteAsync(RentMotorcycleDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.TenantIdentifier))
        {
            return Result<RentDto>.Failure("Identificador do locatário inválido", ResultErrorType.ValidationError);
        }

        if (string.IsNullOrWhiteSpace(dto.MotorcycleIdentifier))
        {
            return Result<RentDto>.Failure("Identificador da moto inválido", ResultErrorType.ValidationError);
        }

        Motorcycle? motorcycle = await motorcycleRepository.GetByIdentifierAsync(dto.MotorcycleIdentifier, cancellationToken);

        if (motorcycle is null)
        {
            return Result<RentDto>.Failure("Moto não encontrada", ResultErrorType.NotFound);
        }

        User? tenant = await userRepository.GetByIdentifierAsync(dto.TenantIdentifier, cancellationToken);

        if (tenant is null)
        {
            return Result<RentDto>.Failure("Usuário locatário não encontrado", ResultErrorType.NotFound);
        }

        Rent? existingRent = await rentRepository.GetActiveRentByMotorcycleAsync(dto.MotorcycleIdentifier, cancellationToken);

        if (existingRent is not null)
        {
            return Result<RentDto>.Failure("Esta moto já está alugada", ResultErrorType.BusinessError);
        }

        Rent rent = new()
        {
            Identifier = Guid.NewGuid().ToString(),
            MotorcycleIdentifier = dto.MotorcycleIdentifier,
            TenantIdentifier = dto.TenantIdentifier,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            EstimatedEndDate = dto.EstimatedEndDate,
            Status = RentStatus.Active
        };

        await rentRepository.AddAsync(rent, cancellationToken);

        RentDto resultDto = new()
        {
            Identifier = rent.Identifier,
            MotorcycleIdentifier = rent.MotorcycleIdentifier,
            TenantIdentifier = rent.TenantIdentifier,
            StartDate = rent.StartDate,
            EndDate = rent.EndDate,
            EstimatedEndDate = rent.EstimatedEndDate,
            Status = rent.Status
        };

        return Result<RentDto>.Success(resultDto);
    }
}
