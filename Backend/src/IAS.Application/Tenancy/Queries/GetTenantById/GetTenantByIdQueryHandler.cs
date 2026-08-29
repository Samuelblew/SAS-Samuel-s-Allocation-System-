using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.Tenancy.Queries.GetTenantById;

public sealed class GetTenantByIdQueryHandler(ITenantRepository repository)
    : IRequestHandler<GetTenantByIdQuery, TenantDto>
{
    public async Task<TenantDto> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        var tenant = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Tenant '{request.Id}' não encontrado.");

        return tenant.ToDto();
    }
}
