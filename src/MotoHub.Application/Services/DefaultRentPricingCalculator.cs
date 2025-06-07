using MotoHub.Application.Interfaces;
using MotoHub.Domain.Entities;

namespace MotoHub.Application.Services;

public class DefaultRentPricingCalculator : IRentPricingCalculator
{
    public decimal CalculateRentalCost(Rent rent, DateOnly rentalEndDate)
    {
        TimeSpan rentalPeriod = (rentalEndDate.ToDateTime(TimeOnly.MinValue) - rent.StartDate);

        int totalDays = (int)Math.Ceiling(rentalPeriod.TotalDays);

        decimal dailyRate = totalDays switch
        {
            >= 50 => 18m,
            >= 45 => 20m,
            >= 30 => 22m,
            >= 15 => 28m,
            >= 7 => 30m,
            _ => 30m,
        };

        return dailyRate * totalDays;
    }
}