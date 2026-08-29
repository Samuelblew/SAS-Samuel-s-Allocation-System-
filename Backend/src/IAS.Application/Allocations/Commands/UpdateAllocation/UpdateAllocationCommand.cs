using IAS.Application.Allocations;
using IAS.Domain.Allocations;
using MediatR;

namespace IAS.Application.Allocations.Commands.UpdateAllocation;

public sealed record UpdateAllocationCommand(
    Guid Id,
    Guid PersonId,
    Guid ProjectId,
    string Role,
    decimal DedicationPercent,
    DateOnly StartDate,
    DateOnly EndDate,
    AllocationStatus Status,
    string? Notes) : IRequest<AllocationDto>, IAllocationCommand;
