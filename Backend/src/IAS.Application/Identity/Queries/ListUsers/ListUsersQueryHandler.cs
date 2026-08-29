using IAS.Application.Common.Models;
using MediatR;

namespace IAS.Application.Identity.Queries.ListUsers;

public sealed class ListUsersQueryHandler(IUserRepository repository)
    : IRequestHandler<ListUsersQuery, PagedResult<UserDto>>
{
    public async Task<PagedResult<UserDto>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await repository.ListAsync(request.Page, request.PageSize, cancellationToken);

        return new PagedResult<UserDto>(
            items.Select(u => u.ToDto()).ToList(),
            request.Page,
            request.PageSize,
            total);
    }
}
