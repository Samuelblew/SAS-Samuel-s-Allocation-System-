using IAS.Application.Common.Models;
using MediatR;

namespace IAS.Application.Identity.Queries.ListUsers;

public sealed record ListUsersQuery(int Page = 1, int PageSize = 20)
    : IRequest<PagedResult<UserDto>>;
