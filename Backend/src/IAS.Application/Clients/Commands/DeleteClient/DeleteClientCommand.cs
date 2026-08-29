using MediatR;

namespace IAS.Application.Clients.Commands.DeleteClient;

public sealed record DeleteClientCommand(Guid Id) : IRequest;
