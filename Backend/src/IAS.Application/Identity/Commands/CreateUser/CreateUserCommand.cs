using IAS.Domain.Identity;
using MediatR;

namespace IAS.Application.Identity.Commands.CreateUser;

public sealed record CreateUserCommand(
    string Email,
    string DisplayName,
    UserRole Role) : IRequest<UserDto>;
