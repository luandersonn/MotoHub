using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces.Repositories;
using MotoHub.Application.Interfaces.UseCases.Renting;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;
using MotoHub.Domain.Interfaces;

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

        if (rent.Status != RentStatus.Active)
        {
            return Result<CompletedRentalDto>.Failure("Este aluguel já foi encerrado", ResultErrorType.BusinessError);
        }

        rent.EndDate = dto.ReturnDate;
        rent.Status = RentStatus.Completed;

        decimal totalCost = pricingCalculator.CalculateRentalCost(rent, dto.ReturnDate);

        await rentRepository.UpdateAsync(rent, cancellationToken);

        CompletedRentalDto resultDto = new()
        {
            Identifier = rent.Identifier,
            MotorcycleIdentifier = rent.MotorcycleIdentifier,
            CourierIdentifier = rent.CourierIdentifier,
            StartDate = rent.StartDate,
            EndDate = rent.EndDate.Value,
            TotalCost = totalCost,
            Status = rent.Status
        };

        return Result<CompletedRentalDto>.Success(resultDto);
    }
}