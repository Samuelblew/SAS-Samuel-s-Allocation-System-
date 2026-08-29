using IAS.Application.Financial;
using IAS.Domain.Projects;

namespace IAS.Application.Tests.Financial;

public sealed class ProjectMarginCalculatorTests
{
    [Fact]
    public void Calculate_ComputesMarginPercent()
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            Name = "Proj",
            EstimatedRevenue = 100_000m,
            CreatedAt = DateTime.UtcNow
        };

        var costs = new[]
        {
            new AllocationCostBreakdown(
                Guid.NewGuid(), Guid.NewGuid(), "A", "Backend", 50m,
                new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 30),
                100m, 4, 80m, 8_000m, true)
        };

        var result = ProjectMarginCalculator.Calculate(project, costs, marginAlertThresholdPercent: 15m);

        Assert.Equal(8_000m, result.TotalCost);
        Assert.Equal(92_000m, result.MarginAmount);
        Assert.Equal(92m, result.MarginPercent);
        Assert.False(result.IsLowMarginAlert);
    }

    [Fact]
    public void Calculate_FlagsLowMarginAlert()
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            Name = "Proj",
            EstimatedRevenue = 100_000m,
            CreatedAt = DateTime.UtcNow
        };

        var costs = new[]
        {
            new AllocationCostBreakdown(
                Guid.NewGuid(), Guid.NewGuid(), "A", "Backend", 100m,
                new DateOnly(2026, 6, 1), new DateOnly(2026, 12, 31),
                100m, 20, 800m, 92_000m, true)
        };

        var result = ProjectMarginCalculator.Calculate(project, costs, marginAlertThresholdPercent: 15m);

        Assert.Equal(8m, result.MarginPercent);
        Assert.True(result.IsLowMarginAlert);
    }
}
