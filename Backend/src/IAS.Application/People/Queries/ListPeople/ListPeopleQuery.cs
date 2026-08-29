using IAS.Application.Common.Models;
using IAS.Application.People;
using MediatR;

namespace IAS.Application.People.Queries.ListPeople;

public sealed record ListPeopleQuery(int Page = 1, int PageSize = 20) : IRequest<PagedResult<PersonListItemDto>>;
