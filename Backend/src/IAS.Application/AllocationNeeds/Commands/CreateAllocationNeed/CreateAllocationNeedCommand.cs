using IAS.Application.AllocationNeeds;
using IAS.Domain.AllocationNeeds;
using MediatR;

namespace IAS.Application.AllocationNeeds.Commands.CreateAllocationNeed;

public sealed record CreateAllocationNeedCommand(
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
