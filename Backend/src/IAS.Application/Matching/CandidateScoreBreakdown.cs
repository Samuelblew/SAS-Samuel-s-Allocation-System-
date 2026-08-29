namespace IAS.Application.Matching;

public sealed record CandidateScoreBreakdown(
    decimal AvailabilityScore,
    decimal RequiredSkillsScore,
    decimal DesiredSkillsScore,
    decimal SeniorityScore,
    decimal HistoryScore,
    decimal CostScore,
    decimal OverloadPenalty,
    decimal SwitchingPenalty,
    decimal TotalScore);

public sealed record RankedCandidate(
    Guid PersonId,
    string PersonName,
    string? JobTitle,
    string? Seniority,
    decimal MinAvailablePercent,
    bool AlreadyOnProject,
    decimal? ProjectDedicationPercent,
    CandidateScoreBreakdown Breakdown);
