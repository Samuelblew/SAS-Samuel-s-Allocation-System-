using IAS.Application.Allocations;
using IAS.Domain.Allocations;
using IAS.Domain.People;

namespace IAS.Application.Financial;

public sealed record AllocationCostBreakdown(
    Guid AllocationId,
    Guid PersonId,
    string PersonName,
    string Role,
    decimal DedicationPercent,
    DateOnly AllocationStart,
    DateOnly AllocationEnd,
    decimal? HourlyRate,
    int WeeksInPeriod,
    decimal TotalHours,
    decimal TotalCost,
    bool HasCostData);

public static class AllocationCostCalculator
{
    public static AllocationCostBreakdown Calculate(
        Allocation allocation,
        Person person,
        DateOnly from,
        DateOnly to)
    {
        if (allocation.Status == AllocationStatus.Closed)
        {
            return Empty(allocation, person);
        }

        var periodStart = allocation.StartDate > from ? allocation.StartDate : from;
        var periodEnd = allocation.EndDate < to ? allocation.EndDate : to;

        if (periodEnd < periodStart)
            return Empty(allocation, person);

        var hourlyRate = PersonCostResolver.ResolveHourlyRate(person);
        var totalHours = 0m;
        var totalCost = 0m;
        var weeks = 0;

        foreach (var (weekStart, weekEnd) in AllocationOverloadChecker.EnumerateWeeks(periodStart, periodEnd))
        {
            if (!Overlaps(allocation.StartDate, allocation.EndDate, weekStart, weekEnd))
                continue;

            weeks++;
            var effectiveHours = person.WeeklyCapacityHours * (allocation.DedicationPercent / 100m);
            totalHours += effectiveHours;

            if (hourlyRate.HasValue)
                totalCost += effectiveHours * hourlyRate.Value;
        }

        return new AllocationCostBreakdown(
            allocation.Id,
            person.Id,
            person.Name,
            allocation.Role,
            allocation.DedicationPercent,
            allocation.StartDate,
            allocation.EndDate,
            hourlyRate,
            weeks,
            Math.Round(totalHours, 2),
            Math.Round(totalCost, 2),
            hourlyRate.HasValue);
    }

    private static AllocationCostBreakdown Empty(Allocation allocation, Person person) =>
        new(
            allocation.Id,
            person.Id,
            person.Name,
            allocation.Role,
            allocation.DedicationPercent,
            allocation.StartDate,
            allocation.EndDate,
            PersonCostResolver.ResolveHourlyRate(person),
            0,
            0,
            0,
            PersonCostResolver.ResolveHourlyRate(person).HasValue);

    private static bool Overlaps(DateOnly aStart, DateOnly aEnd, DateOnly bStart, DateOnly bEnd) =>
        aStart <= bEnd && aEnd >= bStart;
}
