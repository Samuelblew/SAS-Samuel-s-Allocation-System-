using IAS.Application.Capacity;
using IAS.Domain.AllocationNeeds;
using IAS.Domain.Allocations;
using IAS.Domain.People;
using IAS.Domain.Projects;

namespace IAS.Application.Tests.Capacity;

public sealed class FutureCapacityGapsCalculatorTests
{
    [Fact]
    public void Calculate_AggregatesWeeklyDemandAndSupply()
    {
        var projectId = Guid.NewGuid();
        var project = new Project
        {
            Id = projectId,
            TenantId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            Name = "Proj A",
            Status = ProjectStatus.InProgress,
            CreatedAt = DateTime.UtcNow
        };

        AllocationNeed CreateNeed(string role) => new()
        {
            Id = Guid.NewGuid(),
            TenantId = project.TenantId,
            ProjectId = projectId,
            Project = project,
            Role = role,
            DedicationPercent = 100m,
            StartDate = new DateOnly(2026, 6, 1),
            EndDate = new DateOnly(2026, 6, 30),
            Status = AllocationNeedStatus.Open,
            CreatedAt = DateTime.UtcNow
        };

        var needs = new[] { CreateNeed("Backend"), CreateNeed("Frontend") };

        var person = new Person
        {
            Id = Guid.NewGuid(),
            TenantId = project.TenantId,
            Name = "Dev",
            WeeklyCapacityHours = 40,
            Status = PersonStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var data = new CapacityPeriodData([person], [], []);
        var result = FutureCapacityGapsCalculator.Calculate(
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 30),
            needs,
            [],
            data);

        Assert.Equal(2, result.OpenNeeds.Count);
        Assert.NotEmpty(result.Weeks);
        Assert.Contains(result.Weeks, w => w.TotalGapDemandPercent == 200m);
        Assert.Equal(100m, result.PeakShortfallPercent);
    }
}
