using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.Identity.Queries.GetUserById;

public sealed class GetUserByIdQueryHandler(IUserRepository repository)
    : IRequestHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Usuário '{request.Id}' não encontrado.");

        return user.ToDto();
    }
}
