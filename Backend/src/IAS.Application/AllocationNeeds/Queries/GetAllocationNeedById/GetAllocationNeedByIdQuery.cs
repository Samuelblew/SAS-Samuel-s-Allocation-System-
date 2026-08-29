using IAS.Application.AllocationNeeds;
using MediatR;

namespace IAS.Application.AllocationNeeds.Queries.GetAllocationNeedById;

public sealed record GetAllocationNeedByIdQuery(Guid Id) : IRequest<AllocationNeedDto>;
