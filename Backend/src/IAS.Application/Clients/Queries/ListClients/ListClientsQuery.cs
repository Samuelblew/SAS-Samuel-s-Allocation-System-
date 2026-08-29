using IAS.Application.Clients;
using IAS.Application.Common.Models;
using MediatR;

namespace IAS.Application.Clients.Queries.ListClients;

public sealed record ListClientsQuery(int Page = 1, int PageSize = 20) : IRequest<PagedResult<ClientListItemDto>>;
