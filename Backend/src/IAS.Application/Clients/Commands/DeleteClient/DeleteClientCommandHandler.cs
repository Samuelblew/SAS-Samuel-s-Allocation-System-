using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.Clients.Commands.DeleteClient;

public sealed class DeleteClientCommandHandler(IClientRepository repository)
    : IRequestHandler<DeleteClientCommand>
{
    public async Task Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var client = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Cliente '{request.Id}' não encontrado.");

        client.DeletedAt = DateTime.UtcNow;
        client.UpdatedAt = client.DeletedAt;

        await repository.SaveChangesAsync(cancellationToken);
    }
}
