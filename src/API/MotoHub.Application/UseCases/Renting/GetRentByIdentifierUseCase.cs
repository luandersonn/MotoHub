using MotoHub.Application.DTOs;
using MotoHub.Application.Interfaces.Repositories;
using MotoHub.Application.Interfaces.UseCases.Renting;
using MotoHub.Domain.Common;
using MotoHub.Domain.Entities;

namespace MotoHub.Application.UseCases.Renting;

public class GetRentByIdentifierUseCase(IRentRepository rentRepository) : IGetRentByIdentifierUseCase
{
    public async Task<Result<RentDto>> ExecuteAsync(string identifier, CancellationToken cancellationToken = default)
    {
        Rent? rent = await rentRepository.GetByIdAsync(identifier, cancellationToken);

        if (rent is null)
        {
            return Result<RentDto>.Failure("Aluguel não encontrado", ResultErrorType.NotFound);
        }

        RentDto resultDto = new()
        {
            Identifier = rent.Id,
            MotorcycleIdentifier = rent.MotorcycleIdentifier,
            CourierIdentifier = rent.CourierIdentifier,
            StartDate = rent.StartDate,
            EndDate = rent.EndDate,
            EstimatedEndDate = rent.EstimatedEndDate,
            Status = rent.Status,
            DailyRate = rent.DailyRate,
        };

        return Result<RentDto>.Success(resultDto);
    }
}