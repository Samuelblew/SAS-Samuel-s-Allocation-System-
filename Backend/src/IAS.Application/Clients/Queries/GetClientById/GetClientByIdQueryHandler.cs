using IAS.Application.Clients;
using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.Clients.Queries.GetClientById;

public sealed class GetClientByIdQueryHandler(IClientRepository repository)
    : IRequestHandler<GetClientByIdQuery, ClientDto>
{
    public async Task<ClientDto> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Cliente '{request.Id}' não encontrado.");

        return client.ToDto();
    }
}
