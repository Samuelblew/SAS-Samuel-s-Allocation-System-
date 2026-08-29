using MediatR;

namespace IAS.Application.Identity.Commands.DeleteUser;

public sealed record DeleteUserCommand(Guid Id) : IRequest;
