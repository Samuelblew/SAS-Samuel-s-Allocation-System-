using IAS.Domain.AllocationNeeds;

namespace IAS.Application.AllocationNeeds;

public sealed record AllocationNeedDto(
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
