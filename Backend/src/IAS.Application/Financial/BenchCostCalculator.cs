using IAS.Application.Capacity;
using IAS.Domain.People;

namespace IAS.Application.Financial;

public sealed record BenchPersonCost(
    Guid PersonId,
    string PersonName,
    string? Team,
    decimal MinAvailablePercent,
    decimal AvgAvailablePercent,
    decimal BenchHours,
    decimal BenchCost,
    bool HasCostData);

public sealed record BenchCostResult(
    DateOnly From,
    DateOnly To,
    decimal MinAvailablePercent,
    decimal TotalBenchHours,
    decimal TotalBenchCost,
    IReadOnlyList<BenchPersonCost> People);

public static class BenchCostCalculator
{
    public static BenchCostResult Calculate(
        DateOnly from,
        DateOnly to,
        decimal minAvailablePercent,
        CapacityPeriodData data)
    {
        var allocationsByPerson = data.Allocations.GroupBy(a => a.PersonId).ToDictionary(g => g.Key, g => g.ToList());
        var unavailabilitiesByPerson = data.Unavailabilities.GroupBy(u => u.PersonId).ToDictionary(g => g.Key, g => g.ToList());

        var people = new List<BenchPersonCost>();

        foreach (var person in data.People)
        {
            var weeks = PersonAvailabilityCalculator.Calculate(
                from,
                to,
                allocationsByPerson.GetValueOrDefault(person.Id) ?? [],
                unavailabilitiesByPerson.GetValueOrDefault(person.Id) ?? []);

            if (weeks.Count == 0)
                continue;

            var minAvailable = weeks.Min(w => w.AvailablePercent);
            if (minAvailable < minAvailablePercent)
                continue;

            var hourlyRate = PersonCostResolver.ResolveHourlyRate(person);
            var benchHours = 0m;
            var benchCost = 0m;

            foreach (var week in weeks)
            {
                var hours = person.WeeklyCapacityHours * (week.AvailablePercent / 100m);
                benchHours += hours;

                if (hourlyRate.HasValue)
                    benchCost += hours * hourlyRate.Value;
            }

            people.Add(new BenchPersonCost(
                person.Id,
                person.Name,
                person.Team,
                minAvailable,
                Math.Round(weeks.Average(w => w.AvailablePercent), 2),
                Math.Round(benchHours, 2),
                Math.Round(benchCost, 2),
                hourlyRate.HasValue));
        }

        var ordered = people.OrderByDescending(p => p.BenchCost).ThenBy(p => p.PersonName).ToList();

        return new BenchCostResult(
            from,
            to,
            minAvailablePercent,
            Math.Round(ordered.Sum(p => p.BenchHours), 2),
            Math.Round(ordered.Sum(p => p.BenchCost), 2),
            ordered);
    }
}
