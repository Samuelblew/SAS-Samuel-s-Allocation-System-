using IAS.Application.Common.Models;
using MediatR;

namespace IAS.Application.Allocations.Queries.ListAllocations;

public sealed record ListAllocationsQuery(
    int Page,
    int PageSize,
    Guid? PersonId = null,
    Guid? ProjectId = null) : IRequest<PagedResult<AllocationDto>>;
