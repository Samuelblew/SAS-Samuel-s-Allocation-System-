using IAS.Application.Financial;
using IAS.Domain.Allocations;
using IAS.Domain.People;
using IAS.Domain.Projects;

namespace IAS.Application.Tests.Financial;

public sealed class AllocationMarginSimulatorTests
{
    [Fact]
    public void Simulate_ReducesMarginAfterAllocation()
    {
        var tenantId = Guid.NewGuid();
        var project = new Project
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            ClientId = Guid.NewGuid(),
            Name = "Proj",
            EstimatedRevenue = 100_000m,
            CreatedAt = DateTime.UtcNow
        };

        var person = new Person
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = "Dev",
            HourlyCost = 100m,
            WeeklyCapacityHours = 40,
            Status = PersonStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var result = AllocationMarginSimulator.Simulate(
            project,
            [],
            person,
            "Backend",
            50m,
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 30),
            marginAlertThresholdPercent: 15m);

        Assert.True(result.SimulatedAdditionalCost > 0);
        Assert.True(result.ProjectedTotalCost > result.CurrentTotalCost);
        Assert.True(result.MarginDeltaAmount < 0);
        Assert.True(result.ProjectedMarginPercent < result.CurrentMarginPercent);
    }
}
