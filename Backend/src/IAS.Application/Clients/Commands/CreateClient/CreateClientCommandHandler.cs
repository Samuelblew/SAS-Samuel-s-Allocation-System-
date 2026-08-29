using IAS.Application.Clients;
using IAS.Application.Common.Exceptions;
using IAS.Application.Common.Interfaces;
using IAS.Domain.Clients;
using MediatR;

namespace IAS.Application.Clients.Commands.CreateClient;

public sealed class CreateClientCommandHandler(
    IClientRepository repository,
    ITenantContext tenantContext) : IRequestHandler<CreateClientCommand, ClientDto>
{
    public async Task<ClientDto> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        if (!tenantContext.IsResolved)
            throw new InvalidOperationException("Tenant não resolvido.");

        if (await repository.ExistsByNameAsync(request.Name.Trim(), cancellationToken: cancellationToken))
            throw new ConflictException($"Já existe um cliente com o nome '{request.Name}'.");

        var client = new Client
        {
            Id = Guid.NewGuid(),
            TenantId = tenantContext.TenantId,
            Name = request.Name.Trim(),
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await repository.AddAsync(client, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return client.ToDto();
    }
}
