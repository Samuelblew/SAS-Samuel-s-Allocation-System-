using MediatR;

namespace IAS.Application.Matching.Queries.GetAllocationNeedCandidates;

public sealed record GetAllocationNeedCandidatesQuery(
    Guid AllocationNeedId,
    int MaxResults = 20,
    decimal? MinAvailablePercent = null,
    bool ExcludePeopleOnProject = false) : IRequest<AllocationNeedCandidatesDto>;

public sealed record AllocationNeedCandidatesDto(
    Guid AllocationNeedId,
    Guid ProjectId,
    string ProjectName,
    string Role,
    decimal DedicationPercent,
    DateOnly PeriodStart,
    DateOnly PeriodEnd,
    IReadOnlyList<CandidateDto> Candidates);

public sealed record CandidateDto(
    Guid PersonId,
    string PersonName,
    string? JobTitle,
    string? Seniority,
    decimal MinAvailablePercent,
    bool AlreadyOnProject,
    decimal? ProjectDedicationPercent,
    decimal TotalScore,
    CandidateScoreBreakdownDto Breakdown);

public sealed record CandidateScoreBreakdownDto(
    decimal AvailabilityScore,
    decimal RequiredSkillsScore,
    decimal DesiredSkillsScore,
    decimal SeniorityScore,
    decimal HistoryScore,
    decimal CostScore,
    decimal OverloadPenalty,
    decimal SwitchingPenalty,
    decimal TotalScore);
