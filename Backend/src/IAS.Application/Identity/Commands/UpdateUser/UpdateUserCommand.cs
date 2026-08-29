using IAS.Domain.Identity;
using MediatR;

namespace IAS.Application.Identity.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    Guid Id,
    string Email,
    string DisplayName,
    UserRole Role,
    bool IsActive) : IRequest<UserDto>;
