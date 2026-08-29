using IAS.Application.Common.Models;
using IAS.Application.People;
using MediatR;

namespace IAS.Application.People.Queries.ListPeople;

public sealed class ListPeopleQueryHandler(IPersonRepository repository)
    : IRequestHandler<ListPeopleQuery, PagedResult<PersonListItemDto>>
{
    public async Task<PagedResult<PersonListItemDto>> Handle(
        ListPeopleQuery request,
        CancellationToken cancellationToken)
    {
        var (items, total) = await repository.ListAsync(request.Page, request.PageSize, cancellationToken);

        return new PagedResult<PersonListItemDto>(
            items.Select(p => p.ToListItemDto()).ToList(),
            request.Page,
            request.PageSize,
            total);
    }
}
