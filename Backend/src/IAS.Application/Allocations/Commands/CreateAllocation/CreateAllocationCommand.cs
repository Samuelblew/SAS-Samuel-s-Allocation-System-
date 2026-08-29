using IAS.Application.Allocations;
using IAS.Domain.Allocations;
using MediatR;

namespace IAS.Application.Allocations.Commands.CreateAllocation;

public sealed record CreateAllocationCommand(
    Guid PersonId,
    Guid ProjectId,
    string Role,
    decimal DedicationPercent,
    DateOnly StartDate,
    DateOnly EndDate,
    AllocationStatus Status,
    string? Notes) : IRequest<AllocationDto>, IAllocationCommand;
