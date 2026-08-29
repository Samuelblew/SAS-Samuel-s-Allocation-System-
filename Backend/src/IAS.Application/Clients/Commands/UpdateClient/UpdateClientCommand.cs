using IAS.Application.Clients;
using MediatR;

namespace IAS.Application.Clients.Commands.UpdateClient;

public sealed record UpdateClientCommand(Guid Id, string Name, string? Notes) : IRequest<ClientDto>;
