using IAS.Domain.Allocations;

namespace IAS.Application.Allocations;

public static class AllocationOverloadChecker
{
    public static IReadOnlyList<AllocationConflictDto> DetectWeeklyConflicts(
        IReadOnlyList<Allocation> allocations)
    {
        var active = allocations
            .Where(a => a.Status != AllocationStatus.Closed)
            .ToList();

        if (active.Count == 0)
            return [];

        var conflicts = new List<AllocationConflictDto>();

        foreach (var personGroup in active.GroupBy(a => a.PersonId))
        {
            var personAllocations = personGroup.ToList();
            var rangeStart = personAllocations.Min(a => a.StartDate);
            var rangeEnd = personAllocations.Max(a => a.EndDate);

            foreach (var (weekStart, weekEnd) in EnumerateWeeks(rangeStart, rangeEnd))
            {
                var inWeek = personAllocations
                    .Where(a => Overlaps(a.StartDate, a.EndDate, weekStart, weekEnd))
                    .ToList();

                var total = inWeek.Sum(a => a.DedicationPercent);
                if (total <= 100)
                    continue;

                var sample = inWeek[0];
                conflicts.Add(new AllocationConflictDto(
                    sample.PersonId,
                    sample.Person.Name,
                    weekStart,
                    weekEnd,
                    total,
                    inWeek.Select(a => new AllocationConflictItemDto(
                        a.Id,
                        a.ProjectId,
                        a.Project.Name,
                        a.DedicationPercent,
                        a.StartDate,
                        a.EndDate,
                        a.Status)).ToList()));
            }
        }

        return conflicts
            .OrderBy(c => c.WeekStart)
            .ThenBy(c => c.PersonName)
            .ToList();
    }

    public static bool WouldExceedWeeklyCapacity(
        DateOnly start,
        DateOnly end,
        decimal dedicationPercent,
        IReadOnlyList<Allocation> existingAllocations,
        Guid? excludeAllocationId = null) =>
        FindFirstOverloadWeek(start, end, dedicationPercent, existingAllocations, excludeAllocationId) is not null;

    public static WeeklyOverloadDetail? FindFirstOverloadWeek(
        DateOnly start,
        DateOnly end,
        decimal dedicationPercent,
        IReadOnlyList<Allocation> existingAllocations,
        Guid? excludeAllocationId = null)
    {
        foreach (var (weekStart, weekEnd) in EnumerateWeeks(start, end))
        {
            var inWeek = new List<Allocation>();

            foreach (var allocation in existingAllocations)
            {
                if (excludeAllocationId.HasValue && allocation.Id == excludeAllocationId.Value)
                    continue;

                if (allocation.Status == AllocationStatus.Closed)
                    continue;

                if (Overlaps(allocation.StartDate, allocation.EndDate, weekStart, weekEnd))
                    inWeek.Add(allocation);
            }

            var existingTotal = inWeek.Sum(a => a.DedicationPercent);
            var total = dedicationPercent + existingTotal;

            if (total <= 100)
                continue;

            return new WeeklyOverloadDetail(
                weekStart,
                weekEnd,
                dedicationPercent,
                existingTotal,
                total,
                inWeek.Select(a => new AllocationConflictItemDto(
                    a.Id,
                    a.ProjectId,
                    a.Project?.Name ?? a.ProjectId.ToString(),
                    a.DedicationPercent,
                    a.StartDate,
                    a.EndDate,
                    a.Status)).ToList());
        }

        return null;
    }

    internal static IEnumerable<(DateOnly Start, DateOnly End)> EnumerateWeeks(DateOnly start, DateOnly end)
    {
        var cursor = StartOfWeek(start);
        var last = StartOfWeek(end);

        while (cursor <= last)
        {
            yield return (cursor, cursor.AddDays(6));
            cursor = cursor.AddDays(7);
        }
    }

    private static DateOnly StartOfWeek(DateOnly date)
    {
        var offset = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        return date.AddDays(-offset);
    }

    private static bool Overlaps(DateOnly aStart, DateOnly aEnd, DateOnly bStart, DateOnly bEnd) =>
        aStart <= bEnd && aEnd >= bStart;
}
