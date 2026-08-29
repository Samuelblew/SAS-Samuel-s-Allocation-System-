using IAS.Application.Allocations;
using IAS.Domain.Allocations;
using IAS.Domain.Unavailabilities;

namespace IAS.Application.Capacity;

public sealed record WeekAvailability(
    DateOnly WeekStart,
    DateOnly WeekEnd,
    decimal AllocatedPercent,
    decimal AvailablePercent,
    bool IsUnavailable);

public static class PersonAvailabilityCalculator
{
    public static IReadOnlyList<WeekAvailability> Calculate(
        DateOnly from,
        DateOnly to,
        IReadOnlyList<Allocation> allocations,
        IReadOnlyList<Unavailability> unavailabilities)
    {
        var weeks = AllocationOverloadChecker.EnumerateWeeks(from, to).ToList();
        var activeAllocations = allocations.Where(a => a.Status != AllocationStatus.Closed).ToList();

        return weeks.Select(week =>
        {
            var allocated = activeAllocations
                .Where(a => a.StartDate <= week.End && a.EndDate >= week.Start)
                .Sum(a => a.DedicationPercent);

            var unavailable = unavailabilities.Any(u =>
                u.StartDate <= week.End && u.EndDate >= week.Start);

            var available = unavailable ? 0 : Math.Max(0, 100 - allocated);

            return new WeekAvailability(week.Start, week.End, allocated, available, unavailable);
        }).ToList();
    }
}
