using MotoHub.Domain.Interfaces;
using MotoHub.Domain.ValueObjects;

namespace MotoHub.Application.Services;

public class RentPlanCatalog(List<RentPlan> plans) : IRentPlanCatalog
{
    public IReadOnlyDictionary<int, RentPlan> Plans { get; } = plans.ToDictionary(static p => p.PlanNumber, static p => p)
                                                                    .AsReadOnly();

    public RentPlan? FindPlanByNumber(int planNumber)
    {
        Plans.TryGetValue(planNumber, out RentPlan? plan);
        return plan;
    }
    public IReadOnlyList<RentPlan> GetAllPlans() => Plans.Values.ToList().AsReadOnly();
}