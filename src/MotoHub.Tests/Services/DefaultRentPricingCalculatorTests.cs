using MotoHub.Application.Services;
using MotoHub.Domain.Entities;

namespace MotoHub.Tests.Services;

[TestFixture]
public class DefaultRentPricingCalculatorTests
{
    private DefaultRentPricingCalculator _calculator;

    private const decimal DailyRate = 150.00m;
    private const decimal EarlyPenaltyRate = 0.2m;
    private const decimal LateFeeRate = 30.00m;
    private Rent _rent;
    private static readonly DateTime StartDate = new(2025, 1, 1);

    [SetUp]
    public void Setup()
    {
        _calculator = new DefaultRentPricingCalculator();
        _rent = new()
        {
            DailyRate = DailyRate,
            EarlyReturnDailyPenalty = EarlyPenaltyRate,
            LateReturnDailyFee = LateFeeRate,
            StartDate = StartDate
        };
    }

    // Sem atraso nem adiantamento, deve calcular o custo base corretamente
    [Test]
    public void CalculateRentalCost_WithStandardRental_ShouldCalculateBaseCostCorrectly()
    {
        _rent.EstimatedEndDate = StartDate.AddDays(7);
        DateTime returnDate = _rent.EstimatedEndDate;

        int totalDays = (returnDate - _rent.StartDate).Days;
        decimal expectedCost = totalDays * DailyRate;

        decimal cost = _calculator.CalculateRentalCost(_rent, returnDate);

        Assert.That(cost, Is.EqualTo(expectedCost));
    }

    // Com adiantamento, deve aplicar a penalidade com base nos dias não utilizados
    [Test]
    public void CalculateRentalCost_WithEarlyReturn_ShouldApplyPenaltyCorrectly()
    {
        _rent.EstimatedEndDate = StartDate.AddDays(7);
        DateTime returnDate = StartDate.AddDays(5); // Retorno 2 dias antes

        int usedDays = (returnDate - _rent.StartDate).Days;
        int unusedDays = (_rent.EstimatedEndDate - returnDate).Days;

        decimal baseCost = usedDays * DailyRate;
        decimal penalty = unusedDays * DailyRate * EarlyPenaltyRate;
        decimal expectedCost = baseCost + penalty;

        decimal cost = _calculator.CalculateRentalCost(_rent, returnDate);

        Assert.That(cost, Is.EqualTo(expectedCost));
    }

    // Com atraso, deve aplicar a taxa de atraso por dias extras
    [Test]
    public void CalculateRentalCost_WithLateReturn_ShouldApplyLateFeeCorrectly()
    {
        _rent.EstimatedEndDate = StartDate.AddDays(7);
        DateTime returnDate = StartDate.AddDays(9); // Retorno 2 dias depois

        int baseDays = (_rent.EstimatedEndDate - _rent.StartDate).Days;
        int lateDays = (returnDate - _rent.EstimatedEndDate).Days;

        decimal baseCost = baseDays * DailyRate;
        decimal lateFee = lateDays * LateFeeRate;
        decimal expectedCost = baseCost + lateFee;

        decimal cost = _calculator.CalculateRentalCost(_rent, returnDate);

        Assert.That(cost, Is.EqualTo(expectedCost));
    }
    
    [Test]
    public void CalculateRentalCost_WithReturnBeforeStart_ShouldThrowArgumentException()
    {
        _rent.EstimatedEndDate = StartDate.AddDays(7);
        DateTime returnDate = StartDate.AddDays(-5); // Retorno muito antes

        Assert.Throws<ArgumentException>(() => _calculator.CalculateRentalCost(_rent, returnDate));
    }
}