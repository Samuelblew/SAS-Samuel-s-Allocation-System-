using MediatR;

namespace IAS.Application.Identity.Queries.GetUserById;

public sealed record GetUserByIdQuery(Guid Id) : IRequest<UserDto>;
