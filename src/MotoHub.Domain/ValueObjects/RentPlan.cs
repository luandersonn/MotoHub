namespace MotoHub.Domain.ValueObjects;

public class RentPlan
{
    public required int PlanNumber { get; init; }
    public required decimal DailyRate { get; init; }
    public required int DurationInDays { get; init; }
    public required decimal EarlyReturnDailyPenalty { get; init; }
    public required decimal LateReturnDailyFee { get; init; }
}