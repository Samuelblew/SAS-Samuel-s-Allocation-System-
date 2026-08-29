using IAS.Application.AllocationNeeds;
using IAS.Application.Common.Exceptions;
using IAS.Application.Common.Interfaces;
using MediatR;

namespace IAS.Application.AllocationNeeds.Commands.UpdateAllocationNeed;

public sealed class UpdateAllocationNeedCommandHandler(
    IAllocationNeedRepository repository,
    ITenantContext tenantContext) : IRequestHandler<UpdateAllocationNeedCommand, AllocationNeedDto>
{
    public async Task<AllocationNeedDto> Handle(
        UpdateAllocationNeedCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Necessidade de alocação '{request.Id}' não encontrada.");

        await AllocationNeedCommandHandlerBase.ValidateAsync(
            request, repository, tenantContext, cancellationToken);

        AllocationNeedCommandHandlerBase.ApplyToEntity(entity, request);
        entity.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync(cancellationToken);

        var loaded = await repository.GetByIdAsync(entity.Id, cancellationToken)
            ?? throw new InvalidOperationException("Falha ao carregar necessidade atualizada.");

        return loaded.ToDto();
    }
}
