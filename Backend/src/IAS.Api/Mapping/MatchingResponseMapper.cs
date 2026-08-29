using IAS.Api.Dtos;
using IAS.Application.Matching.Queries.GetAllocationNeedCandidates;
using IAS.Application.Matching.Queries.GetProjectMatchingCandidates;

namespace IAS.Api.Mapping;

internal static class MatchingResponseMapper
{
    public static AllocationNeedCandidatesResponse MapNeed(AllocationNeedCandidatesDto dto) =>
        new(
            dto.AllocationNeedId,
            dto.ProjectId,
            dto.ProjectName,
            dto.Role,
            dto.DedicationPercent,
            dto.PeriodStart,
            dto.PeriodEnd,
            dto.Candidates.Select(MapCandidate).ToList());

    public static ProjectMatchingCandidatesResponse MapProject(ProjectMatchingCandidatesDto dto) =>
        new(
            dto.ProjectId,
            dto.ProjectName,
            dto.Needs.Select(MapNeed).ToList());

    private static CandidateMatchResponse MapCandidate(CandidateDto c) =>
        new(
            c.PersonId,
            c.PersonName,
            c.JobTitle,
            c.Seniority,
            c.MinAvailablePercent,
            c.AlreadyOnProject,
            c.ProjectDedicationPercent,
            c.TotalScore,
            new CandidateScoreBreakdownResponse(
                c.Breakdown.AvailabilityScore,
                c.Breakdown.RequiredSkillsScore,
                c.Breakdown.DesiredSkillsScore,
                c.Breakdown.SeniorityScore,
                c.Breakdown.HistoryScore,
                c.Breakdown.CostScore,
                c.Breakdown.OverloadPenalty,
                c.Breakdown.SwitchingPenalty,
                c.Breakdown.TotalScore));
}
