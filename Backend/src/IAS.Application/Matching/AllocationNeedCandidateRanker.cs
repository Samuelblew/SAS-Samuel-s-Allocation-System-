using IAS.Application.Capacity;
using IAS.Application.Matching.Queries.GetAllocationNeedCandidates;
using IAS.Domain.AllocationNeeds;

namespace IAS.Application.Matching;

public static class AllocationNeedCandidateRanker
{
    public static async Task<CapacityPeriodData> LoadCapacityDataAsync(
        ICapacityReadRepository capacityRepository,
        DateOnly periodStart,
        DateOnly periodEnd,
        CancellationToken cancellationToken)
    {
        var historyStart = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-12));
        var loadFrom = periodStart < historyStart ? periodStart : historyStart;

        var people = await capacityRepository.ListActivePeopleWithSkillsAsync(cancellationToken);
        var allocations = await capacityRepository.GetAllocationsInPeriodAsync(loadFrom, periodEnd, cancellationToken);
        var unavailabilities = await capacityRepository.GetUnavailabilitiesInPeriodAsync(
            periodStart, periodEnd, cancellationToken);

        return new CapacityPeriodData(people, allocations, unavailabilities);
    }

    public static AllocationNeedCandidatesDto Rank(
        AllocationNeed need,
        CapacityPeriodData data,
        int maxResults,
        CandidateMatchFilters? filters = null)
    {
        var (periodStart, periodEnd) = AllocationNeedCandidateMatcher.ResolvePeriod(need);
        var ranked = AllocationNeedCandidateMatcher.Rank(
            need,
            need.Project.ProjectType,
            data,
            maxResults,
            filters);

        return new AllocationNeedCandidatesDto(
            need.Id,
            need.ProjectId,
            need.Project.Name,
            need.Role,
            need.DedicationPercent,
            periodStart,
            periodEnd,
            ranked.Select(Map).ToList());
    }

    private static CandidateDto Map(RankedCandidate candidate) =>
        new(
            candidate.PersonId,
            candidate.PersonName,
            candidate.JobTitle,
            candidate.Seniority,
            candidate.MinAvailablePercent,
            candidate.AlreadyOnProject,
            candidate.ProjectDedicationPercent,
            candidate.Breakdown.TotalScore,
            new CandidateScoreBreakdownDto(
                candidate.Breakdown.AvailabilityScore,
                candidate.Breakdown.RequiredSkillsScore,
                candidate.Breakdown.DesiredSkillsScore,
                candidate.Breakdown.SeniorityScore,
                candidate.Breakdown.HistoryScore,
                candidate.Breakdown.CostScore,
                candidate.Breakdown.OverloadPenalty,
                candidate.Breakdown.SwitchingPenalty,
                candidate.Breakdown.TotalScore));
}
