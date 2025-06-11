using MotoHub.Application.Services;
using MotoHub.Domain.ValueObjects;

namespace MotoHub.Tests.Services;

[TestFixture]
public class RentPlanCatalogTests
{
    private RentPlanCatalog _catalog;

    [SetUp]
    public void Setup()
    {
        List<RentPlan> plans =
        [
            new RentPlan
            {
                PlanNumber = 1,
                DailyRate = 150.00m,
                DurationInDays = 7,
                EarlyReturnDailyPenalty = 30.00m,
                LateReturnDailyFee = 20.00m
            },
            new RentPlan
            {
                PlanNumber = 2,
                DailyRate = 200.00m,
                DurationInDays = 14,
                EarlyReturnDailyPenalty = 40.00m,
                LateReturnDailyFee = 25.00m
            }
        ];

        _catalog = new RentPlanCatalog(plans);
    }

    [Test]
    public void Constructor_ShouldInitializePlansCorrectly()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_catalog.Plans, Has.Count.EqualTo(2));
            Assert.That(_catalog.Plans.ContainsKey(1), Is.True);
            Assert.That(_catalog.Plans.ContainsKey(2), Is.True);
        });
    }

    [Test]
    public void FindPlanByNumber_WithExistingPlan_ShouldReturnCorrectPlan()
    {
        RentPlan? plan = _catalog.FindPlanByNumber(1);

        Assert.Multiple(() =>
        {
            Assert.That(plan, Is.Not.Null);
            Assert.That(plan!.PlanNumber, Is.EqualTo(1));
            Assert.That(plan.DailyRate, Is.EqualTo(150.00m));
        });
    }

    [Test]
    public void FindPlanByNumber_WithNonExistingPlan_ShouldReturnNull()
    {
        RentPlan? plan = _catalog.FindPlanByNumber(99);

        Assert.That(plan, Is.Null);
    }

    [Test]
    public void GetAllPlans_ShouldReturnAllPlansCorrectly()
    {
        IReadOnlyList<RentPlan> plans = _catalog.GetAllPlans();

        Assert.Multiple(() =>
        {
            Assert.That(plans, Has.Count.EqualTo(2));
            Assert.That(plans.Any(p => p.PlanNumber == 1), Is.True);
            Assert.That(plans.Any(p => p.PlanNumber == 2), Is.True);
        });
    }
}