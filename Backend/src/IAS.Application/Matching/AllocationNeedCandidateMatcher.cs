using IAS.Application.Allocations;
using IAS.Application.Capacity;
using IAS.Domain.AllocationNeeds;
using IAS.Domain.Allocations;
using IAS.Domain.People;

namespace IAS.Application.Matching;

public static class AllocationNeedCandidateMatcher
{
    public const decimal MaxAvailability = 25m;
    public const decimal MaxRequiredSkills = 25m;
    public const decimal MaxDesiredSkills = 10m;
    public const decimal MaxSeniority = 15m;
    public const decimal MaxHistory = 10m;
    public const decimal MaxCost = 10m;
    public const decimal MaxOverloadPenalty = 20m;
    public const decimal MaxSwitchingPenalty = 10m;

    public static IReadOnlyList<RankedCandidate> Rank(
        AllocationNeed need,
        string? projectType,
        CapacityPeriodData data,
        int maxResults = 20,
        CandidateMatchFilters? filters = null)
    {
        var (periodStart, periodEnd) = ResolvePeriod(need);
        var allocationsByPerson = data.Allocations.GroupBy(a => a.PersonId).ToDictionary(g => g.Key, g => g.ToList());
        var unavailabilitiesByPerson = data.Unavailabilities.GroupBy(u => u.PersonId).ToDictionary(g => g.Key, g => g.ToList());

        var costBaselines = data.People
            .Where(p => p.HourlyCost.HasValue || p.MonthlyCost.HasValue)
            .Select(p => p.HourlyCost ?? (p.MonthlyCost!.Value / 160m))
            .ToList();

        var candidates = new List<RankedCandidate>();

        foreach (var person in data.People)
        {
            var personAllocations = allocationsByPerson.GetValueOrDefault(person.Id) ?? [];
            var weeks = PersonAvailabilityCalculator.Calculate(
                periodStart,
                periodEnd,
                personAllocations,
                unavailabilitiesByPerson.GetValueOrDefault(person.Id) ?? []);

            var minAvailable = weeks.Count == 0 ? 0 : weeks.Min(w => w.AvailablePercent);

            if (filters?.MinAvailablePercent is decimal minRequired && minAvailable < minRequired)
                continue;

            var onProjectAllocation = personAllocations.FirstOrDefault(a =>
                a.ProjectId == need.ProjectId
                && a.Status != AllocationStatus.Closed
                && a.StartDate <= periodEnd
                && a.EndDate >= periodStart);

            var alreadyOnProject = onProjectAllocation is not null;

            if (filters?.ExcludePeopleOnProject == true && alreadyOnProject)
                continue;

            var breakdown = BuildBreakdown(
                need,
                projectType,
                person,
                personAllocations,
                minAvailable,
                costBaselines);

            candidates.Add(new RankedCandidate(
                person.Id,
                person.Name,
                person.JobTitle,
                person.Seniority,
                minAvailable,
                alreadyOnProject,
                onProjectAllocation?.DedicationPercent,
                breakdown));
        }

        return candidates
            .OrderByDescending(c => c.Breakdown.TotalScore)
            .ThenByDescending(c => c.MinAvailablePercent)
            .Take(maxResults)
            .ToList();
    }

    internal static (DateOnly Start, DateOnly End) ResolvePeriod(AllocationNeed need)
    {
        var start = need.StartDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var end = need.EndDate ?? start.AddMonths(3);
        if (end < start)
            end = start.AddMonths(3);

        return (start, end);
    }

    private static CandidateScoreBreakdown BuildBreakdown(
        AllocationNeed need,
        string? projectType,
        Person person,
        IReadOnlyList<Allocation> personAllocations,
        decimal minAvailablePercent,
        IReadOnlyList<decimal> costBaselines)
    {
        var availability = ScoreAvailability(minAvailablePercent, need.DedicationPercent);
        var requiredSkills = ScoreRequiredSkills(person, need.RequiredSkillIds);
        var desiredSkills = ScoreDesiredSkills(person, need.DesiredSkillIds);
        var seniority = ScoreSeniority(person.Seniority, need.ExpectedSeniority);
        var history = ScoreHistory(personAllocations, need.Role, projectType);
        var cost = ScoreCost(person, costBaselines);
        var overloadPenalty = ScoreOverloadPenalty(
            need, personAllocations, person.Id, ResolvePeriod(need));
        var switchingPenalty = ScoreSwitchingPenalty(personAllocations);

        var total = Math.Max(
            0,
            availability + requiredSkills + desiredSkills + seniority + history + cost
            - overloadPenalty - switchingPenalty);

        return new CandidateScoreBreakdown(
            availability,
            requiredSkills,
            desiredSkills,
            seniority,
            history,
            cost,
            overloadPenalty,
            switchingPenalty,
            Math.Round(total, 2));
    }

    private static decimal ScoreAvailability(decimal minAvailable, decimal requiredDedication)
    {
        if (requiredDedication <= 0)
            return 0;

        var ratio = Math.Min(1, minAvailable / requiredDedication);
        return Math.Round(ratio * MaxAvailability, 2);
    }

    private static decimal ScoreRequiredSkills(Person person, IReadOnlyList<Guid> requiredSkillIds)
    {
        if (requiredSkillIds.Count == 0)
            return MaxRequiredSkills;

        var personSkills = person.Skills.Select(s => s.SkillId).ToHashSet();
        var matched = requiredSkillIds.Count(personSkills.Contains);
        return Math.Round((decimal)matched / requiredSkillIds.Count * MaxRequiredSkills, 2);
    }

    private static decimal ScoreDesiredSkills(Person person, IReadOnlyList<Guid> desiredSkillIds)
    {
        if (desiredSkillIds.Count == 0)
            return 0;

        var personSkills = person.Skills.Select(s => s.SkillId).ToHashSet();
        var matched = desiredSkillIds.Count(personSkills.Contains);
        return Math.Round((decimal)matched / desiredSkillIds.Count * MaxDesiredSkills, 2);
    }

    private static decimal ScoreSeniority(string? personSeniority, string? expectedSeniority)
    {
        if (string.IsNullOrWhiteSpace(expectedSeniority))
            return MaxSeniority;

        if (string.IsNullOrWhiteSpace(personSeniority))
            return 0;

        if (personSeniority.Equals(expectedSeniority, StringComparison.OrdinalIgnoreCase))
            return MaxSeniority;

        if (personSeniority.Contains(expectedSeniority, StringComparison.OrdinalIgnoreCase)
            || expectedSeniority.Contains(personSeniority, StringComparison.OrdinalIgnoreCase))
            return MaxSeniority * 0.6m;

        return 0;
    }

    private static decimal ScoreHistory(
        IReadOnlyList<Allocation> allocations,
        string needRole,
        string? projectType)
    {
        var score = 0m;
        var active = allocations.Where(a => a.Status != AllocationStatus.Closed).ToList();

        if (active.Any(a => RolesMatch(a.Role, needRole)))
            score += MaxHistory * 0.5m;

        if (!string.IsNullOrWhiteSpace(projectType)
            && active.Any(a => string.Equals(a.Project?.ProjectType, projectType, StringComparison.OrdinalIgnoreCase)))
        {
            score += MaxHistory * 0.5m;
        }

        return Math.Round(score, 2);
    }

    private static decimal ScoreCost(Person person, IReadOnlyList<decimal> costBaselines)
    {
        if (costBaselines.Count == 0)
            return MaxCost * 0.5m;

        var personCost = person.HourlyCost ?? (person.MonthlyCost.HasValue ? person.MonthlyCost.Value / 160m : (decimal?)null);
        if (!personCost.HasValue)
            return MaxCost * 0.5m;

        var min = costBaselines.Min();
        var max = costBaselines.Max();
        if (max <= min)
            return MaxCost;

        var normalized = 1 - (personCost.Value - min) / (max - min);
        return Math.Round(Math.Clamp(normalized, 0, 1) * MaxCost, 2);
    }

    private static decimal ScoreOverloadPenalty(
        AllocationNeed need,
        IReadOnlyList<Allocation> personAllocations,
        Guid personId,
        (DateOnly Start, DateOnly End) period)
    {
        if (AllocationOverloadChecker.WouldExceedWeeklyCapacity(
                period.Start,
                period.End,
                need.DedicationPercent,
                personAllocations,
                excludeAllocationId: null))
        {
            return MaxOverloadPenalty;
        }

        var weeks = PersonAvailabilityCalculator.Calculate(
            period.Start, period.End, personAllocations, []);

        var maxAllocated = weeks.Count == 0 ? 0 : weeks.Max(w => w.AllocatedPercent);
        if (maxAllocated + need.DedicationPercent > 100)
            return Math.Round(MaxOverloadPenalty * 0.5m, 2);

        return 0;
    }

    private static decimal ScoreSwitchingPenalty(IReadOnlyList<Allocation> personAllocations)
    {
        var cutoff = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-12));
        var recentProjects = personAllocations
            .Where(a => a.EndDate >= cutoff && a.Status != AllocationStatus.Closed)
            .Select(a => a.ProjectId)
            .Distinct()
            .Count();

        if (recentProjects <= 2)
            return 0;
        if (recentProjects == 3)
            return MaxSwitchingPenalty * 0.5m;

        return MaxSwitchingPenalty;
    }

    private static bool RolesMatch(string allocationRole, string needRole) =>
        string.Equals(allocationRole.Trim(), needRole.Trim(), StringComparison.OrdinalIgnoreCase);
}

public sealed record CandidateMatchFilters(
    decimal? MinAvailablePercent = null,
    bool ExcludePeopleOnProject = false);
