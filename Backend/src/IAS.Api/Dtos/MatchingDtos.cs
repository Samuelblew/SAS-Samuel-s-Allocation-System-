namespace IAS.Api.Dtos;

public sealed record AllocationNeedCandidatesResponse(
    Guid AllocationNeedId,
    Guid ProjectId,
    string ProjectName,
    string Role,
    decimal DedicationPercent,
    DateOnly PeriodStart,
    DateOnly PeriodEnd,
    IReadOnlyList<CandidateMatchResponse> Candidates);

public sealed record CandidateMatchResponse(
    Guid PersonId,
    string PersonName,
    string? JobTitle,
    string? Seniority,
    decimal MinAvailablePercent,
    bool AlreadyOnProject,
    decimal? ProjectDedicationPercent,
    decimal TotalScore,
    CandidateScoreBreakdownResponse Breakdown);

public sealed record ProjectMatchingCandidatesResponse(
    Guid ProjectId,
    string ProjectName,
    IReadOnlyList<AllocationNeedCandidatesResponse> Needs);

public sealed record CandidateScoreBreakdownResponse(
    decimal AvailabilityScore,
    decimal RequiredSkillsScore,
    decimal DesiredSkillsScore,
    decimal SeniorityScore,
    decimal HistoryScore,
    decimal CostScore,
    decimal OverloadPenalty,
    decimal SwitchingPenalty,
    decimal TotalScore);
