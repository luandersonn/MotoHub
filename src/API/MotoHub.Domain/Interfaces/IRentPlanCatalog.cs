using MotoHub.Domain.ValueObjects;

namespace MotoHub.Domain.Interfaces;

public interface IRentPlanCatalog
{
    IReadOnlyList<RentPlan> GetAllPlans();
    RentPlan? FindPlanByNumber(int planNumber);
}