using IAS.Application.AllocationNeeds;
using IAS.Application.Common.Models;
using MediatR;

namespace IAS.Application.AllocationNeeds.Queries.ListAllocationNeeds;

public sealed record ListAllocationNeedsQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? ProjectId = null) : IRequest<PagedResult<AllocationNeedDto>>;
