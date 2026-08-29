using IAS.Application.Clients;
using IAS.Application.Common.Models;
using MediatR;

namespace IAS.Application.Clients.Queries.ListClients;

public sealed class ListClientsQueryHandler(IClientRepository repository)
    : IRequestHandler<ListClientsQuery, PagedResult<ClientListItemDto>>
{
    public async Task<PagedResult<ClientListItemDto>> Handle(
        ListClientsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, total) = await repository.ListAsync(request.Page, request.PageSize, cancellationToken);

        return new PagedResult<ClientListItemDto>(
            items.Select(c => c.ToListItemDto()).ToList(),
            request.Page,
            request.PageSize,
            total);
    }
}
