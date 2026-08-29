using IAS.Application.AllocationNeeds;
using IAS.Domain.AllocationNeeds;
using MediatR;

namespace IAS.Application.AllocationNeeds.Commands.UpdateAllocationNeed;

public sealed record UpdateAllocationNeedCommand(
    Guid Id,
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
    AllocationNeedStatus Status) : IRequest<AllocationNeedDto>, IAllocationNeedCommand;
