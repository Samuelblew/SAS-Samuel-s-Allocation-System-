using IAS.Domain.AllocationNeeds;

namespace IAS.Api.Dtos;

public sealed record CreateAllocationNeedRequest(
    Guid ProjectId,
    string Role,
    string? ExpectedSeniority,
    IReadOnlyList<Guid> RequiredSkillIds,
    IReadOnlyList<Guid> DesiredSkillIds,
    decimal DedicationPercent,
    DateOnly? StartDate,
    DateOnly? EndDate,
    AllocationNeedUrgency Urgency,
    AllocationNeedCriticality Criticality,
    AllocationNeedStatus Status);

public sealed record UpdateAllocationNeedRequest(
    Guid ProjectId,
    string Role,
    string? ExpectedSeniority,
    IReadOnlyList<Guid> RequiredSkillIds,
    IReadOnlyList<Guid> DesiredSkillIds,
    decimal DedicationPercent,
    DateOnly? StartDate,
    DateOnly? EndDate,
    AllocationNeedUrgency Urgency,
    AllocationNeedCriticality Criticality,
    AllocationNeedStatus Status);

public sealed record AllocationNeedResponse(
    Guid Id,
    Guid ProjectId,
    string ProjectName,
    string Role,
    string? ExpectedSeniority,
    IReadOnlyList<Guid> RequiredSkillIds,
    IReadOnlyList<Guid> DesiredSkillIds,
    decimal DedicationPercent,
    DateOnly? StartDate,
    DateOnly? EndDate,
    AllocationNeedUrgency Urgency,
    AllocationNeedCriticality Criticality,
    AllocationNeedStatus Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record AllocationNeedListItemResponse(
    Guid Id,
    Guid ProjectId,
    string ProjectName,
    string Role,
    string? ExpectedSeniority,
    IReadOnlyList<Guid> RequiredSkillIds,
    decimal DedicationPercent,
    DateOnly? StartDate,
    DateOnly? EndDate,
    AllocationNeedUrgency Urgency,
    AllocationNeedCriticality Criticality,
    AllocationNeedStatus Status,
    DateTime CreatedAt);

public sealed record PagedAllocationNeedsResponse(
    IReadOnlyList<AllocationNeedListItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
