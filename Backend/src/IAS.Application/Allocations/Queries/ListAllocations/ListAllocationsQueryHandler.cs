using IAS.Application.Allocations;
using IAS.Application.Common.Models;
using MediatR;

namespace IAS.Application.Allocations.Queries.ListAllocations;

public sealed class ListAllocationsQueryHandler(IAllocationRepository repository)
    : IRequestHandler<ListAllocationsQuery, PagedResult<AllocationDto>>
{
    public async Task<PagedResult<AllocationDto>> Handle(
        ListAllocationsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, total) = await repository.ListAsync(
            request.Page,
            request.PageSize,
            request.PersonId,
            request.ProjectId,
            cancellationToken);

        return new PagedResult<AllocationDto>(
            items.Select(i => i.ToDto()).ToList(),
            request.Page,
            request.PageSize,
            total);
    }
}
