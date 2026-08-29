using IAS.Application.AllocationNeeds;
using IAS.Application.Allocations;
using IAS.Domain.AllocationNeeds;
using IAS.Domain.Allocations;

namespace IAS.Application.Capacity;

public sealed record NeedGapSnapshot(
    Guid NeedId,
    Guid ProjectId,
    string ProjectName,
    string Role,
    decimal RequiredPercent,
    decimal CoveredPercent,
    decimal GapPercent,
    AllocationNeedStatus Status,
    DateOnly? StartDate,
    DateOnly? EndDate);

public sealed record WeekCapacityGap(
    DateOnly WeekStart,
    DateOnly WeekEnd,
    decimal TotalGapDemandPercent,
    decimal TotalAvailableSupplyPercent,
    decimal NetShortfallPercent,
    int OpenNeedsInWeek);

public sealed record FutureCapacityGapsResult(
    DateOnly From,
    DateOnly To,
    decimal PeakShortfallPercent,
    IReadOnlyList<WeekCapacityGap> Weeks,
    IReadOnlyList<NeedGapSnapshot> OpenNeeds);

public static class FutureCapacityGapsCalculator
{
    public static FutureCapacityGapsResult Calculate(
        DateOnly from,
        DateOnly to,
        IReadOnlyList<AllocationNeed> needs,
        IReadOnlyList<Allocation> allocations,
        CapacityPeriodData capacityData)
    {
        var allocationsByProject = allocations.GroupBy(a => a.ProjectId).ToDictionary(g => g.Key, g => g.ToList());
        var openNeeds = new List<NeedGapSnapshot>();

        foreach (var need in needs)
        {
            var projectAllocations = allocationsByProject.GetValueOrDefault(need.ProjectId) ?? [];
            var covered = AllocationNeedStatusCalculator.CalculateCoveredPercent(need, projectAllocations);
            var status = AllocationNeedStatusCalculator.ResolveStatus(covered, need.DedicationPercent);
            var gap = Math.Max(0, need.DedicationPercent - covered);

            if (status == AllocationNeedStatus.Filled)
                continue;

            openNeeds.Add(new NeedGapSnapshot(
                need.Id,
                need.ProjectId,
                need.Project.Name,
                need.Role,
                need.DedicationPercent,
                covered,
                gap,
                status,
                need.StartDate,
                need.EndDate));
        }

        var allocationsByPerson = capacityData.Allocations.GroupBy(a => a.PersonId).ToDictionary(g => g.Key, g => g.ToList());
        var unavailabilitiesByPerson = capacityData.Unavailabilities.GroupBy(u => u.PersonId).ToDictionary(g => g.Key, g => g.ToList());

        var availabilityByPerson = capacityData.People.ToDictionary(
            p => p.Id,
            p => PersonAvailabilityCalculator.Calculate(
                from,
                to,
                allocationsByPerson.GetValueOrDefault(p.Id) ?? [],
                unavailabilitiesByPerson.GetValueOrDefault(p.Id) ?? []));

        var weeks = new List<WeekCapacityGap>();
        var peakShortfall = 0m;

        foreach (var (weekStart, weekEnd) in AllocationOverloadChecker.EnumerateWeeks(from, to))
        {
            var demand = openNeeds
                .Where(n => OverlapsWeek(n.StartDate, n.EndDate, weekStart, weekEnd))
                .Sum(n => n.GapPercent);

            var supply = availabilityByPerson.Values
                .Select(personWeeks => personWeeks.FirstOrDefault(w => w.WeekStart == weekStart))
                .Where(w => w is not null)
                .Sum(w => w!.AvailablePercent);

            var shortfall = Math.Max(0, demand - supply);
            peakShortfall = Math.Max(peakShortfall, shortfall);

            weeks.Add(new WeekCapacityGap(
                weekStart,
                weekEnd,
                Math.Round(demand, 2),
                Math.Round(supply, 2),
                Math.Round(shortfall, 2),
                openNeeds.Count(n => OverlapsWeek(n.StartDate, n.EndDate, weekStart, weekEnd))));
        }

        return new FutureCapacityGapsResult(
            from,
            to,
            Math.Round(peakShortfall, 2),
            weeks,
            openNeeds.OrderByDescending(n => n.GapPercent).ThenBy(n => n.ProjectName).ToList());
    }

    internal static bool OverlapsWeek(DateOnly? needStart, DateOnly? needEnd, DateOnly weekStart, DateOnly weekEnd)
    {
        if (!needStart.HasValue && !needEnd.HasValue)
            return true;

        var rangeStart = needStart ?? DateOnly.MinValue;
        var rangeEnd = needEnd ?? DateOnly.MaxValue;

        return rangeStart <= weekEnd && rangeEnd >= weekStart;
    }
}
