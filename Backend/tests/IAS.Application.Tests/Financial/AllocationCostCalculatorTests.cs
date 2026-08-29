using IAS.Application.Financial;
using IAS.Domain.Allocations;
using IAS.Domain.People;

namespace IAS.Application.Tests.Financial;

public sealed class AllocationCostCalculatorTests
{
    [Fact]
    public void Calculate_ComputesCostFromDedicationAndHourlyRate()
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            Name = "Dev",
            HourlyCost = 100m,
            WeeklyCapacityHours = 40,
            Status = PersonStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var allocation = new Allocation
        {
            Id = Guid.NewGuid(),
            TenantId = person.TenantId,
            PersonId = person.Id,
            ProjectId = Guid.NewGuid(),
            Role = "Backend",
            DedicationPercent = 50m,
            StartDate = new DateOnly(2026, 6, 2),
            EndDate = new DateOnly(2026, 6, 29),
            Status = AllocationStatus.Confirmed,
            CreatedAt = DateTime.UtcNow
        };

        var result = AllocationCostCalculator.Calculate(
            allocation,
            person,
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 30));

        Assert.True(result.WeeksInPeriod >= 4);
        Assert.Equal(20m * result.WeeksInPeriod, result.TotalHours);
        Assert.Equal(100m * result.TotalHours, result.TotalCost);
        Assert.True(result.HasCostData);
    }

    [Fact]
    public void Calculate_ReturnsZeroForClosedAllocation()
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            Name = "Dev",
            HourlyCost = 100m,
            WeeklyCapacityHours = 40,
            Status = PersonStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var allocation = new Allocation
        {
            Id = Guid.NewGuid(),
            TenantId = person.TenantId,
            PersonId = person.Id,
            ProjectId = Guid.NewGuid(),
            Role = "Backend",
            DedicationPercent = 100m,
            StartDate = new DateOnly(2026, 6, 1),
            EndDate = new DateOnly(2026, 6, 30),
            Status = AllocationStatus.Closed,
            CreatedAt = DateTime.UtcNow
        };

        var result = AllocationCostCalculator.Calculate(
            allocation,
            person,
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 30));

        Assert.Equal(0, result.TotalHours);
        Assert.Equal(0, result.TotalCost);
    }
}
