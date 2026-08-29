using IAS.Application.AllocationNeeds;
using IAS.Application.Common.Models;
using MediatR;

namespace IAS.Application.AllocationNeeds.Queries.ListAllocationNeeds;

public sealed class ListAllocationNeedsQueryHandler(IAllocationNeedRepository repository)
    : IRequestHandler<ListAllocationNeedsQuery, PagedResult<AllocationNeedDto>>
{
    public async Task<PagedResult<AllocationNeedDto>> Handle(
        ListAllocationNeedsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, total) = await repository.ListAsync(
            request.Page,
            request.PageSize,
            request.ProjectId,
            cancellationToken);

        return new PagedResult<AllocationNeedDto>(
            items.Select(i => i.ToDto()).ToList(),
            request.Page,
            request.PageSize,
            total);
    }
}
