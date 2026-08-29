using IAS.Domain.Allocations;
using IAS.Domain.Projects;

namespace IAS.Application.Financial;

public static class FinancialPeriodResolver
{
    public static (DateOnly From, DateOnly To) Resolve(
        Project project,
        IReadOnlyList<Allocation> allocations,
        DateOnly? from,
        DateOnly? to)
    {
        if (from.HasValue && to.HasValue)
            return (from.Value, to.Value);

        var start = from
            ?? project.StartDate
            ?? (allocations.Count > 0 ? allocations.Min(a => a.StartDate) : DateOnly.FromDateTime(DateTime.UtcNow));

        var end = to
            ?? project.EndDate
            ?? (allocations.Count > 0 ? allocations.Max(a => a.EndDate) : start.AddMonths(3));

        if (end < start)
            end = start.AddMonths(3);

        return (start, end);
    }
}
