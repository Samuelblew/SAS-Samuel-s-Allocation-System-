using IAS.Application.AllocationNeeds;
using IAS.Domain.AllocationNeeds;
using IAS.Domain.Allocations;

namespace IAS.Application.Tests.AllocationNeeds;

public sealed class AllocationNeedStatusCalculatorTests
{
    private static readonly Guid ProjectId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public void Calculate_Open_WhenNoMatchingAllocations()
    {
        var need = CreateNeed("Backend", 50);
        var status = AllocationNeedStatusCalculator.Calculate(need, []);
        Assert.Equal(AllocationNeedStatus.Open, status);
    }

    [Fact]
    public void Calculate_PartiallyFilled_WhenCoveredBelowRequired()
    {
        var need = CreateNeed("Backend", 80);
        var allocations = new[] { CreateAllocation("Backend", 40) };
        var status = AllocationNeedStatusCalculator.Calculate(need, allocations);
        Assert.Equal(AllocationNeedStatus.PartiallyFilled, status);
    }

    [Fact]
    public void Calculate_Filled_WhenCoveredMeetsRequired()
    {
        var need = CreateNeed("Backend", 50);
        var allocations = new[]
        {
            CreateAllocation("Backend", 30),
            CreateAllocation("Backend", 20)
        };
        var status = AllocationNeedStatusCalculator.Calculate(need, allocations);
        Assert.Equal(AllocationNeedStatus.Filled, status);
    }

    [Fact]
    public void Calculate_IgnoresClosedAllocations()
    {
        var need = CreateNeed("Backend", 50);
        var allocation = CreateAllocation("Backend", 50);
        allocation.Status = AllocationStatus.Closed;
        var status = AllocationNeedStatusCalculator.Calculate(need, [allocation]);
        Assert.Equal(AllocationNeedStatus.Open, status);
    }

    private static AllocationNeed CreateNeed(string role, decimal dedication) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            ProjectId = ProjectId,
            Role = role,
            DedicationPercent = dedication,
            CreatedAt = DateTime.UtcNow
        };

    private static Allocation CreateAllocation(string role, decimal dedication) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            ProjectId = ProjectId,
            PersonId = Guid.NewGuid(),
            Role = role,
            DedicationPercent = dedication,
            StartDate = new DateOnly(2026, 6, 1),
            EndDate = new DateOnly(2026, 12, 31),
            Status = AllocationStatus.Confirmed,
            CreatedAt = DateTime.UtcNow
        };
}
