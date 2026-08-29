namespace IAS.Application.Capacity;

file sealed record SkillPersonSample(
    string SkillName,
    string? Category,
    IReadOnlyList<WeekAvailability> Weeks,
    decimal WeeklyHours);

public sealed record SkillOccupation(
    Guid SkillId,
    string SkillName,
    string? Category,
    int PeopleCount,
    decimal AvgAllocatedPercent,
    decimal AvgAvailablePercent,
    decimal AvgAllocatedHours,
    decimal AvgAvailableHours);

public static class SkillOccupationCalculator
{
    public static IReadOnlyList<SkillOccupation> Calculate(
        DateOnly from,
        DateOnly to,
        CapacityPeriodData data)
    {
        if (data.People.Count == 0)
            return [];

        var allocationsByPerson = data.Allocations.GroupBy(a => a.PersonId).ToDictionary(g => g.Key, g => g.ToList());
        var unavailabilitiesByPerson = data.Unavailabilities.GroupBy(u => u.PersonId).ToDictionary(g => g.Key, g => g.ToList());

        var skillPeople = new Dictionary<Guid, List<SkillPersonSample>>();

        foreach (var person in data.People)
        {
            if (person.Skills.Count == 0)
                continue;

            var weeks = PersonAvailabilityCalculator.Calculate(
                from,
                to,
                allocationsByPerson.GetValueOrDefault(person.Id) ?? [],
                unavailabilitiesByPerson.GetValueOrDefault(person.Id) ?? []);

            foreach (var personSkill in person.Skills)
            {
                if (!skillPeople.TryGetValue(personSkill.SkillId, out var entries))
                {
                    entries = [];
                    skillPeople[personSkill.SkillId] = entries;
                }

                entries.Add(new SkillPersonSample(
                    personSkill.Skill?.Name ?? string.Empty,
                    personSkill.Skill?.Category,
                    weeks,
                    person.WeeklyCapacityHours));
            }
        }

        return skillPeople
            .Select(group =>
            {
                var first = group.Value[0];
                var skillName = first.SkillName;
                var category = first.Category;
                var allocatedPercents = new List<decimal>();
                var availablePercents = new List<decimal>();
                var allocatedHours = new List<decimal>();
                var availableHours = new List<decimal>();

                foreach (var entry in group.Value)
                {
                    allocatedPercents.AddRange(entry.Weeks.Select(w => w.AllocatedPercent));
                    availablePercents.AddRange(entry.Weeks.Select(w => w.AvailablePercent));

                    foreach (var week in entry.Weeks)
                    {
                        var hours = EffectiveCapacityCalculator.FromWeek(week, entry.WeeklyHours);
                        allocatedHours.Add(hours.AllocatedHours);
                        availableHours.Add(hours.AvailableHours);
                    }
                }

                return new SkillOccupation(
                    group.Key,
                    skillName,
                    category,
                    group.Value.Count,
                    allocatedPercents.Count == 0 ? 0 : Math.Round(allocatedPercents.Average(), 2),
                    availablePercents.Count == 0 ? 0 : Math.Round(availablePercents.Average(), 2),
                    allocatedHours.Count == 0 ? 0 : Math.Round(allocatedHours.Average(), 2),
                    availableHours.Count == 0 ? 0 : Math.Round(availableHours.Average(), 2));
            })
            .OrderBy(s => s.SkillName)
            .ToList();
    }
}
