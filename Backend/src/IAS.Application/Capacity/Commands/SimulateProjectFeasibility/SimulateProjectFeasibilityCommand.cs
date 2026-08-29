using MediatR;

namespace IAS.Application.Capacity.Commands.SimulateProjectFeasibility;

public sealed record SimulateProjectFeasibilityCommand(
    DateOnly DesiredStartDate,
    int DurationMonths,
    IReadOnlyList<SimulatedNeedInput> Needs) : IRequest<ProjectFeasibilityDto>;

public sealed record SimulatedNeedInput(
    string Role,
    string? ExpectedSeniority,
    IReadOnlyList<Guid> RequiredSkillIds,
    decimal DedicationPercent,
    int Quantity);

public sealed record ProjectFeasibilityDto(
    DateOnly DesiredStartDate,
    DateOnly SimulatedEndDate,
    bool FeasibleAtDesiredStart,
    DateOnly? EarliestFeasibleStart,
    int WeeksScanned,
    int ActivePeopleCount,
    int BenchAtDesiredStart,
    int TotalHeadcountRequired,
    IReadOnlyList<RoleFeasibilityDto> Roles);

public sealed record RoleFeasibilityDto(
    string Role,
    string? ExpectedSeniority,
    decimal DedicationPercent,
    int QuantityRequired,
    int CandidatesAtDesiredStart,
    bool SatisfiedAtDesiredStart,
    IReadOnlyList<RoleCandidatePreviewDto> EligibleCandidates);

public sealed record RoleCandidatePreviewDto(
    Guid PersonId,
    string PersonName,
    string? Seniority,
    decimal MinAvailablePercent);
