using MediatR;

namespace IAS.Application.Allocations.Commands.DeleteAllocation;

public sealed record DeleteAllocationCommand(Guid Id) : IRequest;
