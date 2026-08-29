using IAS.Application.AllocationNeeds;
using IAS.Application.Common.Interfaces;
using IAS.Domain.AllocationNeeds;
using MediatR;

namespace IAS.Application.AllocationNeeds.Commands.CreateAllocationNeed;

public sealed class CreateAllocationNeedCommandHandler(
    IAllocationNeedRepository repository,
    ITenantContext tenantContext) : IRequestHandler<CreateAllocationNeedCommand, AllocationNeedDto>
{
    public async Task<AllocationNeedDto> Handle(
        CreateAllocationNeedCommand request,
        CancellationToken cancellationToken)
    {
        await AllocationNeedCommandHandlerBase.ValidateAsync(
            request, repository, tenantContext, cancellationToken);

        var entity = new AllocationNeed
        {
            Id = Guid.NewGuid(),
            TenantId = tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        AllocationNeedCommandHandlerBase.ApplyToEntity(entity, request);

        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        var loaded = await repository.GetByIdAsync(entity.Id, cancellationToken)
            ?? throw new InvalidOperationException("Falha ao carregar necessidade criada.");

        return loaded.ToDto();
    }
}
