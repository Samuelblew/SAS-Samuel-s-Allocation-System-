using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.Identity.Commands.DeleteUser;

public sealed class DeleteUserCommandHandler(IUserRepository repository)
    : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Usuário '{request.Id}' não encontrado.");

        user.DeletedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync(cancellationToken);
    }
}
