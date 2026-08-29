namespace IAS.Api.Dtos;

public sealed record SimulateProjectFeasibilityRequest(
    DateOnly DesiredStartDate,
    int DurationMonths,
    IReadOnlyList<SimulatedNeedRequest> Needs);

public sealed record SimulatedNeedRequest(
    string Role,
    string? ExpectedSeniority,
    IReadOnlyList<Guid> RequiredSkillIds,
    decimal DedicationPercent,
    int Quantity);

public sealed record ProjectFeasibilityResponse(
    DateOnly DesiredStartDate,
    DateOnly SimulatedEndDate,
    bool FeasibleAtDesiredStart,
    DateOnly? EarliestFeasibleStart,
    int WeeksScanned,
    int ActivePeopleCount,
    int BenchAtDesiredStart,
    int TotalHeadcountRequired,
    IReadOnlyList<RoleFeasibilityResponse> Roles);

public sealed record RoleFeasibilityResponse(
    string Role,
    string? ExpectedSeniority,
    decimal DedicationPercent,
    int QuantityRequired,
    int CandidatesAtDesiredStart,
    bool SatisfiedAtDesiredStart,
    IReadOnlyList<RoleCandidatePreviewResponse> EligibleCandidates);

public sealed record RoleCandidatePreviewResponse(
    Guid PersonId,
    string PersonName,
    string? Seniority,
    decimal MinAvailablePercent);

public sealed record SimulateAllocationMarginRequest(
    Guid ProjectId,
    Guid PersonId,
    string Role,
    decimal DedicationPercent,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal MarginAlertThresholdPercent = 15m);

public sealed record AllocationMarginSimulationResponse(
    Guid ProjectId,
    string ProjectName,
    DateOnly PeriodStart,
    DateOnly PeriodEnd,
    decimal? CurrentTotalCost,
    decimal? CurrentMarginAmount,
    decimal? CurrentMarginPercent,
    decimal SimulatedAdditionalCost,
    decimal? ProjectedTotalCost,
    decimal? ProjectedMarginAmount,
    decimal? ProjectedMarginPercent,
    decimal MarginDeltaAmount,
    decimal? MarginDeltaPercent,
    bool HasRevenueData,
    bool CurrentIsLowMarginAlert,
    bool ProjectedIsLowMarginAlert,
    decimal MarginAlertThresholdPercent);
