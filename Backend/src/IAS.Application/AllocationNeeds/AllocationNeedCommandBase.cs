using IAS.Domain.AllocationNeeds;

namespace IAS.Application.AllocationNeeds;

public interface IAllocationNeedCommand
{
    Guid ProjectId { get; }
    string Role { get; }
    string? ExpectedSeniority { get; }
    IReadOnlyList<Guid> RequiredSkillIds { get; }
    IReadOnlyList<Guid> DesiredSkillIds { get; }
    decimal DedicationPercent { get; }
    DateOnly? StartDate { get; }
    DateOnly? EndDate { get; }
    AllocationNeedUrgency Urgency { get; }
    AllocationNeedCriticality Criticality { get; }
    AllocationNeedStatus Status { get; }
}
