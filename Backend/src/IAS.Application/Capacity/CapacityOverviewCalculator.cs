using IAS.Application.Allocations;

namespace IAS.Application.Capacity;

public sealed record WeekOverview(
    DateOnly WeekStart,
    DateOnly WeekEnd,
    int ActivePeopleCount,
    decimal AvgAllocatedPercent,
    decimal AvgAvailablePercent,
    int BenchPeopleCount,
    int OverallocatedPeopleCount,
    decimal TotalCapacityHours,
    decimal TotalAllocatedHours,
    decimal TotalAvailableHours);

public sealed record TeamOccupation(
    string? Team,
    int PeopleCount,
    decimal AvgAllocatedPercent,
    decimal AvgAvailablePercent);

public static class CapacityOverviewCalculator
{
    public const decimal DefaultBenchThreshold = 50m;

    public static IReadOnlyList<WeekOverview> CalculateWeeklyOverview(
        DateOnly from,
        DateOnly to,
        CapacityPeriodData data,
        decimal benchThreshold = DefaultBenchThreshold)
    {
        if (data.People.Count == 0)
            return [];

        var allocationsByPerson = data.Allocations.GroupBy(a => a.PersonId).ToDictionary(g => g.Key, g => g.ToList());
        var unavailabilitiesByPerson = data.Unavailabilities.GroupBy(u => u.PersonId).ToDictionary(g => g.Key, g => g.ToList());

        var availabilityByPerson = data.People.ToDictionary(
            p => p.Id,
            p => (
                p.WeeklyCapacityHours,
                Weeks: PersonAvailabilityCalculator.Calculate(
                    from, to,
                    allocationsByPerson.GetValueOrDefault(p.Id) ?? [],
                    unavailabilitiesByPerson.GetValueOrDefault(p.Id) ?? [])));

        var weekStarts = AllocationOverloadChecker.EnumerateWeeks(from, to).Select(w => w.Start).ToList();

        return weekStarts.Select(weekStart =>
        {
            var weekSamples = availabilityByPerson.Values
                .Select(entry => entry.Weeks.FirstOrDefault(w => w.WeekStart == weekStart))
                .Where(w => w is not null)
                .Select(w => w!)
                .ToList();

            if (weekSamples.Count == 0)
            {
                return new WeekOverview(
                    weekStart, weekStart.AddDays(6), 0, 0, 0, 0, 0, 0, 0, 0);
            }

            var totalCapacityHours = 0m;
            var totalAllocatedHours = 0m;
            var totalAvailableHours = 0m;

            foreach (var (weeklyCapacityHours, weeks) in availabilityByPerson.Values)
            {
                var week = weeks.FirstOrDefault(w => w.WeekStart == weekStart);
                if (week is null)
                    continue;

                totalCapacityHours += weeklyCapacityHours;
                var hours = EffectiveCapacityCalculator.FromWeek(week, weeklyCapacityHours);
                totalAllocatedHours += hours.AllocatedHours;
                totalAvailableHours += hours.AvailableHours;
            }

            return new WeekOverview(
                weekSamples[0].WeekStart,
                weekSamples[0].WeekEnd,
                weekSamples.Count,
                Math.Round(weekSamples.Average(w => w.AllocatedPercent), 2),
                Math.Round(weekSamples.Average(w => w.AvailablePercent), 2),
                weekSamples.Count(w => w.AvailablePercent >= benchThreshold && !w.IsUnavailable),
                weekSamples.Count(w => w.AllocatedPercent > 100),
                Math.Round(totalCapacityHours, 2),
                Math.Round(totalAllocatedHours, 2),
                Math.Round(totalAvailableHours, 2));
        }).ToList();
    }

    public static IReadOnlyList<TeamOccupation> CalculateTeamOccupation(
        DateOnly from,
        DateOnly to,
        CapacityPeriodData data)
    {
        var allocationsByPerson = data.Allocations.GroupBy(a => a.PersonId).ToDictionary(g => g.Key, g => g.ToList());
        var unavailabilitiesByPerson = data.Unavailabilities.GroupBy(u => u.PersonId).ToDictionary(g => g.Key, g => g.ToList());

        return data.People
            .GroupBy(p => p.Team)
            .Select(group =>
            {
                var allocatedSamples = new List<decimal>();
                var availableSamples = new List<decimal>();

                foreach (var person in group)
                {
                    var weeks = PersonAvailabilityCalculator.Calculate(
                        from, to,
                        allocationsByPerson.GetValueOrDefault(person.Id) ?? [],
                        unavailabilitiesByPerson.GetValueOrDefault(person.Id) ?? []);

                    allocatedSamples.AddRange(weeks.Select(w => w.AllocatedPercent));
                    availableSamples.AddRange(weeks.Select(w => w.AvailablePercent));
                }

                return new TeamOccupation(
                    group.Key,
                    group.Count(),
                    allocatedSamples.Count == 0 ? 0 : Math.Round(allocatedSamples.Average(), 2),
                    availableSamples.Count == 0 ? 0 : Math.Round(availableSamples.Average(), 2));
            })
            .OrderBy(t => t.Team)
            .ToList();
    }
}
