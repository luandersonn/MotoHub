using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces;
using MotoHub.Application.Interfaces.UseCases.Renting;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;
using MotoHub.Domain.Interfaces;
using MotoHub.Domain.ValueObjects;

namespace MotoHub.Application.UseCases.Renting;

public class RentMotorcycleUseCase(IRentRepository rentRepository,
                                   IMotorcycleRepository motorcycleRepository,
                                   IUserRepository userRepository,
                                   IRentPlanCatalog rentPlanCatalog) : IRentMotorcycleUseCase
{
    public async Task<Result<RentDto>> ExecuteAsync(RentMotorcycleDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Identifier))
        {
            dto.Identifier = Guid.NewGuid().ToString();
        }
        else
        {
            if (await rentRepository.GetByIdentifierAsync(dto.Identifier, cancellationToken) is not null)
            {
                return Result<RentDto>.Failure("Identificador de locação já existe", ResultErrorType.ValidationError);
            }
        }

        if (string.IsNullOrWhiteSpace(dto.CourierIdentifier))
        {
            return Result<RentDto>.Failure("Identificador do locatário inválido", ResultErrorType.ValidationError);
        }

        if (string.IsNullOrWhiteSpace(dto.MotorcycleIdentifier))
        {
            return Result<RentDto>.Failure("Identificador da moto inválido", ResultErrorType.ValidationError);
        }

        RentPlan? plan = rentPlanCatalog.FindPlanByNumber(dto.Plan);

        if (plan is null)
        {
            return Result<RentDto>.Failure("Plano de aluguel inválido", ResultErrorType.ValidationError);
        }

        Motorcycle? motorcycle = await motorcycleRepository.GetByIdentifierAsync(dto.MotorcycleIdentifier, cancellationToken);

        if (motorcycle is null)
        {
            return Result<RentDto>.Failure("Moto não encontrada", ResultErrorType.NotFound);
        }

        User? courier = await userRepository.GetByIdentifierAsync(dto.CourierIdentifier, cancellationToken);

        if (courier is null)
        {
            return Result<RentDto>.Failure("Entregador com identificador informado não encontrado", ResultErrorType.NotFound);
        }

        if (courier.DriverLicenseType is not DriverLicenseType.A and not DriverLicenseType.AB)
        {
            return Result<RentDto>.Failure("O entregador não tem o tipo de licença A para realizar o aluguel", ResultErrorType.BusinessError);
        }

        Rent? existingRent = await rentRepository.GetActiveRentByMotorcycleAsync(dto.MotorcycleIdentifier, cancellationToken);

        if (existingRent is not null)
        {
            return Result<RentDto>.Failure("Esta moto já está alugada", ResultErrorType.BusinessError);
        }

        DateTime startDate = DateTime.Now.AddDays(1).Date;
        DateTime estimatedEndDate = startDate.AddDays(plan.DurationInDays).Date;

        Rent rent = new()
        {
            Identifier = dto.Identifier,
            MotorcycleIdentifier = dto.MotorcycleIdentifier,
            CourierIdentifier = dto.CourierIdentifier,
            StartDate = startDate,
            EndDate = null,
            EstimatedEndDate = estimatedEndDate,
            Status = RentStatus.Active,
            DailyRate = plan.DailyRate,
            EarlyReturnDailyPenalty = plan.EarlyReturnDailyPenalty,
            LateReturnDailyFee = plan.LateReturnDailyFee
        };

        await rentRepository.AddAsync(rent, cancellationToken);

        RentDto resultDto = new()
        {
            Identifier = rent.Identifier,
            MotorcycleIdentifier = rent.MotorcycleIdentifier,
            CourierIdentifier = rent.CourierIdentifier,
            StartDate = rent.StartDate,
            EndDate = rent.EndDate,
            EstimatedEndDate = rent.EstimatedEndDate,
            Plan = plan.PlanNumber,
            DailyRate = plan.DailyRate,
            Status = rent.Status
        };

        return Result<RentDto>.Success(resultDto);
    }
}
