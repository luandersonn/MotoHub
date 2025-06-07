using MotoHub.Domain.Entities;

namespace MotoHub.Application.Interfaces;

public interface IRentPricingCalculator
{
    decimal CalculateRentalCost(Rent rent, DateOnly rentalEndDate);
}