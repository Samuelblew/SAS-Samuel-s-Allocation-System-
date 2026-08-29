using IAS.Application.Allocations;
using MediatR;

namespace IAS.Application.Allocations.Queries.GetAllocationById;

public sealed record GetAllocationByIdQuery(Guid Id) : IRequest<AllocationDto>;
