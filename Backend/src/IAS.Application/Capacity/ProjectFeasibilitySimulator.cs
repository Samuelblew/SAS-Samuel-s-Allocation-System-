using IAS.Domain.People;

namespace IAS.Application.Capacity;

public sealed record SimulatedNeed(
    string Role,
    string? ExpectedSeniority,
    IReadOnlyList<Guid> RequiredSkillIds,
    decimal DedicationPercent,
    int Quantity);

public sealed record RoleCandidatePreview(
    Guid PersonId,
    string PersonName,
    string? Seniority,
    decimal MinAvailablePercent);

public sealed record RoleFeasibilityResult(
    string Role,
    string? ExpectedSeniority,
    decimal DedicationPercent,
    int QuantityRequired,
    int CandidatesAtDesiredStart,
    bool SatisfiedAtDesiredStart,
    IReadOnlyList<RoleCandidatePreview> EligibleCandidates);

public sealed record ProjectFeasibilityResult(
    DateOnly DesiredStartDate,
    DateOnly SimulatedEndDate,
    bool FeasibleAtDesiredStart,
    DateOnly? EarliestFeasibleStart,
    int WeeksScanned,
    int ActivePeopleCount,
    int BenchAtDesiredStart,
    int TotalHeadcountRequired,
    IReadOnlyList<RoleFeasibilityResult> Roles);

public static class ProjectFeasibilitySimulator
{
    public static ProjectFeasibilityResult Simulate(
        DateOnly desiredStart,
        int durationMonths,
        IReadOnlyList<SimulatedNeed> needs,
        CapacityPeriodData data,
        int maxWeeksToScan = 26)
    {
        var endDate = desiredStart.AddMonths(durationMonths);
        var roleResultsAtDesired = EvaluateRoles(desiredStart, endDate, needs, data.People, data);
        var feasibleAtDesired = roleResultsAtDesired.All(r => r.SatisfiedAtDesiredStart);
        var benchAtDesired = CountBench(desiredStart, endDate, data);

        DateOnly? earliest = feasibleAtDesired ? desiredStart : null;
        var weeksScanned = 1;

        if (!feasibleAtDesired)
        {
            for (var week = 1; week < maxWeeksToScan; week++)
            {
                var candidateStart = desiredStart.AddDays(week * 7);
                var candidateEnd = candidateStart.AddMonths(durationMonths);
                var roleResults = EvaluateRoles(candidateStart, candidateEnd, needs, data.People, data);

                weeksScanned = week + 1;
                if (roleResults.All(r => r.SatisfiedAtDesiredStart))
                {
                    earliest = candidateStart;
                    roleResultsAtDesired = roleResults;
                    break;
                }
            }
        }

        return new ProjectFeasibilityResult(
            desiredStart,
            endDate,
            feasibleAtDesired,
            earliest,
            weeksScanned,
            data.People.Count,
            benchAtDesired,
            needs.Sum(n => n.Quantity),
            roleResultsAtDesired);
    }

    private static int CountBench(
        DateOnly start,
        DateOnly end,
        CapacityPeriodData data)
    {
        var allocationsByPerson = data.Allocations.GroupBy(a => a.PersonId).ToDictionary(g => g.Key, g => g.ToList());
        var unavailabilitiesByPerson = data.Unavailabilities.GroupBy(u => u.PersonId).ToDictionary(g => g.Key, g => g.ToList());
        var bench = 0;

        foreach (var person in data.People)
        {
            var weeks = PersonAvailabilityCalculator.Calculate(
                start, end,
                allocationsByPerson.GetValueOrDefault(person.Id) ?? [],
                unavailabilitiesByPerson.GetValueOrDefault(person.Id) ?? []);

            if (weeks.Count == 0)
                continue;

            if (weeks.Min(w => w.AvailablePercent) >= 50)
                bench++;
        }

        return bench;
    }

    private static IReadOnlyList<RoleFeasibilityResult> EvaluateRoles(
        DateOnly start,
        DateOnly end,
        IReadOnlyList<SimulatedNeed> needs,
        IReadOnlyList<Person> people,
        CapacityPeriodData data)
    {
        var allocationsByPerson = data.Allocations.GroupBy(a => a.PersonId).ToDictionary(g => g.Key, g => g.ToList());
        var unavailabilitiesByPerson = data.Unavailabilities.GroupBy(u => u.PersonId).ToDictionary(g => g.Key, g => g.ToList());

        var results = new List<RoleFeasibilityResult>();

        foreach (var need in needs)
        {
            var eligible = new List<RoleCandidatePreview>();

            foreach (var person in people)
            {
                if (!MatchesNeed(person, need))
                    continue;

                var weeks = PersonAvailabilityCalculator.Calculate(
                    start, end,
                    allocationsByPerson.GetValueOrDefault(person.Id) ?? [],
                    unavailabilitiesByPerson.GetValueOrDefault(person.Id) ?? []);

                if (weeks.Count == 0)
                    continue;

                var minAvailable = weeks.Min(w => w.AvailablePercent);
                if (minAvailable >= need.DedicationPercent)
                {
                    eligible.Add(new RoleCandidatePreview(
                        person.Id,
                        person.Name,
                        person.Seniority,
                        minAvailable));
                }
            }

            results.Add(new RoleFeasibilityResult(
                need.Role,
                need.ExpectedSeniority,
                need.DedicationPercent,
                need.Quantity,
                eligible.Count,
                eligible.Count >= need.Quantity,
                eligible.OrderByDescending(c => c.MinAvailablePercent).ToList()));
        }

        return results;
    }

    private static bool MatchesNeed(Person person, SimulatedNeed need)
    {
        if (need.RequiredSkillIds.Count > 0)
        {
            var personSkillIds = person.Skills.Select(s => s.SkillId).ToHashSet();
            if (!need.RequiredSkillIds.All(personSkillIds.Contains))
                return false;
        }

        if (!string.IsNullOrWhiteSpace(need.ExpectedSeniority))
        {
            if (string.IsNullOrWhiteSpace(person.Seniority))
                return false;

            if (!SeniorityMatches(person.Seniority, need.ExpectedSeniority))
                return false;
        }

        return true;
    }

    private static bool SeniorityMatches(string personSeniority, string expectedSeniority)
    {
        var person = NormalizeSeniority(personSeniority);
        var expected = NormalizeSeniority(expectedSeniority);
        return person == expected;
    }

    private static string NormalizeSeniority(string value)
    {
        var normalized = value.Trim().ToLowerInvariant();
        return normalized switch
        {
            "estagiario" or "estagiário" => "estagiario",
            "junior" or "júnior" => "junior",
            "pleno" => "pleno",
            "senior" or "sênior" => "senior",
            _ => normalized
        };
    }
}
