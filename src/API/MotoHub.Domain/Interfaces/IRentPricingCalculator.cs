using MotoHub.Domain.Entities;

namespace MotoHub.Domain.Interfaces;

public interface IRentPricingCalculator
{
    decimal CalculateRentalCost(Rent rent, DateTime rentalEndDate);
}