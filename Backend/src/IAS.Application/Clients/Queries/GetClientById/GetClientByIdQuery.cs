using IAS.Application.Clients;
using MediatR;

namespace IAS.Application.Clients.Queries.GetClientById;

public sealed record GetClientByIdQuery(Guid Id) : IRequest<ClientDto>;
