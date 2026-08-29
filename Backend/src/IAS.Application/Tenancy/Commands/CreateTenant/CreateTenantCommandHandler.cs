using IAS.Application.Common.Exceptions;
using IAS.Domain.Tenancy;
using MediatR;

namespace IAS.Application.Tenancy.Commands.CreateTenant;

public sealed class CreateTenantCommandHandler(ITenantRepository repository)
    : IRequestHandler<CreateTenantCommand, TenantDto>
{
    public async Task<TenantDto> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        if (await repository.ExistsByNameAsync(name, cancellationToken))
            throw new ConflictException($"Já existe um tenant com o nome '{name}'.");

        var now = DateTime.UtcNow;
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = name,
            IsActive = true,
            CreatedAt = now
        };

        await repository.AddAsync(tenant, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return tenant.ToDto();
    }
}
