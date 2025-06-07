using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces;
using MotoHub.Application.Interfaces.UseCases.Renting;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;

namespace MotoHub.Application.UseCases.Renting;

public class ReturnMotorcycleUseCase(IRentRepository rentRepository, IRentPricingCalculator pricingCalculator) : IReturnMotorcycleUseCase
{
    public async Task<Result<CompletedRentalDto>> ExecuteAsync(ReturnMotorcycleDto dto, CancellationToken cancellationToken = default)
    {
        Rent? rent = await rentRepository.GetByIdentifierAsync(dto.RentIdentifier, cancellationToken);

        if (rent is null)
        {
            return Result<CompletedRentalDto>.Failure("Aluguel não encontrado", ResultErrorType.NotFound);
        }

        if (dto.ReturnDate < rent.StartDate)
        {
            return Result<CompletedRentalDto>.Failure("A data de devolução não pode ser anterior à data de início do aluguel", ResultErrorType.ValidationError);
        }

        if (rent.Status != RentStatus.Active)
        {
            return Result<CompletedRentalDto>.Failure("Este aluguel já foi encerrado", ResultErrorType.BusinessError);
        }

        rent.EndDate = dto.ReturnDate;
        rent.Status = RentStatus.Completed;

        decimal totalCost = pricingCalculator.CalculateRentalCost(rent, DateOnly.FromDateTime(dto.ReturnDate));

        await rentRepository.UpdateAsync(rent, cancellationToken);

        CompletedRentalDto resultDto = new()
        {
            Identifier = rent.Identifier,
            MotorcycleIdentifier = rent.MotorcycleIdentifier,
            TenantIdentifier = rent.TenantIdentifier,
            StartDate = rent.StartDate,
            EndDate = rent.EndDate.Value,
            TotalCost = totalCost,
            Status = rent.Status
        };

        return Result<CompletedRentalDto>.Success(resultDto);
    }
}