using IAS.Domain.AllocationNeeds;
using IAS.Domain.Allocations;

namespace IAS.Application.AllocationNeeds;

public static class AllocationNeedStatusCalculator
{
    public static decimal CalculateCoveredPercent(
        AllocationNeed need,
        IEnumerable<Allocation> allocations)
    {
        return allocations
            .Where(a => a.ProjectId == need.ProjectId
                && a.Status != AllocationStatus.Closed
                && RolesMatch(a.Role, need.Role)
                && DateRangesOverlap(a.StartDate, a.EndDate, need.StartDate, need.EndDate))
            .Sum(a => a.DedicationPercent);
    }

    public static AllocationNeedStatus ResolveStatus(decimal coveredPercent, decimal requiredPercent)
    {
        if (coveredPercent >= requiredPercent)
            return AllocationNeedStatus.Filled;

        if (coveredPercent > 0)
            return AllocationNeedStatus.PartiallyFilled;

        return AllocationNeedStatus.Open;
    }

    public static AllocationNeedStatus Calculate(
        AllocationNeed need,
        IEnumerable<Allocation> allocations) =>
        ResolveStatus(CalculateCoveredPercent(need, allocations), need.DedicationPercent);

    private static bool RolesMatch(string allocationRole, string needRole) =>
        string.Equals(allocationRole.Trim(), needRole.Trim(), StringComparison.OrdinalIgnoreCase);

    private static bool DateRangesOverlap(
        DateOnly allocationStart,
        DateOnly allocationEnd,
        DateOnly? needStart,
        DateOnly? needEnd)
    {
        if (!needStart.HasValue && !needEnd.HasValue)
            return true;

        var rangeStart = needStart ?? DateOnly.MinValue;
        var rangeEnd = needEnd ?? DateOnly.MaxValue;

        return allocationStart <= rangeEnd && allocationEnd >= rangeStart;
    }
}
