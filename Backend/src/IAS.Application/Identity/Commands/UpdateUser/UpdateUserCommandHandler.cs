using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.Identity.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler(IUserRepository repository)
    : IRequestHandler<UpdateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Usuário '{request.Id}' não encontrado.");

        var email = request.Email.Trim().ToLowerInvariant();
        if (await repository.ExistsByEmailAsync(email, request.Id, cancellationToken))
            throw new ConflictException($"Já existe um usuário com o e-mail '{email}'.");

        user.Email = email;
        user.DisplayName = request.DisplayName.Trim();
        user.Role = request.Role;
        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync(cancellationToken);

        return user.ToDto();
    }
}
