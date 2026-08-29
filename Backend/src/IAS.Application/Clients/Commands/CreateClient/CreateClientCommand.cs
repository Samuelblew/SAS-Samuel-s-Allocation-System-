using IAS.Application.Clients;
using MediatR;

namespace IAS.Application.Clients.Commands.CreateClient;

public sealed record CreateClientCommand(string Name, string? Notes) : IRequest<ClientDto>;
