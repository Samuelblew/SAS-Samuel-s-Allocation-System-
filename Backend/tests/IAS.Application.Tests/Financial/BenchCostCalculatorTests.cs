using IAS.Application.Capacity;
using IAS.Application.Financial;
using IAS.Domain.People;

namespace IAS.Application.Tests.Financial;

public sealed class BenchCostCalculatorTests
{
    [Fact]
    public void Calculate_ComputesBenchHoursAndCost()
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            Name = "Bench Dev",
            HourlyCost = 100m,
            WeeklyCapacityHours = 40,
            Status = PersonStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var data = new CapacityPeriodData([person], [], []);
        var result = BenchCostCalculator.Calculate(
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 30),
            minAvailablePercent: 50m,
            data);

        Assert.Single(result.People);
        Assert.True(result.TotalBenchHours > 0);
        Assert.True(result.TotalBenchCost > 0);
        Assert.Equal(result.People[0].BenchHours * 100m, result.People[0].BenchCost);
    }
}
