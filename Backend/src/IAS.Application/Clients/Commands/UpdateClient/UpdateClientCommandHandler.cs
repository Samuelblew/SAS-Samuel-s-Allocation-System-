using IAS.Application.Clients;
using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.Clients.Commands.UpdateClient;

public sealed class UpdateClientCommandHandler(IClientRepository repository)
    : IRequestHandler<UpdateClientCommand, ClientDto>
{
    public async Task<ClientDto> Handle(UpdateClientCommand request, CancellationToken cancellationToken)
    {
        var client = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Cliente '{request.Id}' não encontrado.");

        if (await repository.ExistsByNameAsync(request.Name.Trim(), request.Id, cancellationToken))
            throw new ConflictException($"Já existe um cliente com o nome '{request.Name}'.");

        client.Name = request.Name.Trim();
        client.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();
        client.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync(cancellationToken);

        return client.ToDto();
    }
}
