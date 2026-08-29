using MediatR;

namespace IAS.Application.AllocationNeeds.Commands.DeleteAllocationNeed;

public sealed record DeleteAllocationNeedCommand(Guid Id) : IRequest;
