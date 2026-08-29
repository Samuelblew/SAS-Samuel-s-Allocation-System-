using IAS.Application.AllocationNeeds;
using IAS.Application.Allocations;
using IAS.Application.Common.Exceptions;
using IAS.Application.Common.Interfaces;
using MediatR;

namespace IAS.Application.Allocations.Commands.UpdateAllocation;

public sealed class UpdateAllocationCommandHandler(
    IAllocationRepository repository,
    IAllocationNeedStatusSync needStatusSync,
    ITenantContext tenantContext) : IRequestHandler<UpdateAllocationCommand, AllocationDto>
{
    public async Task<AllocationDto> Handle(
        UpdateAllocationCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Alocação '{request.Id}' não encontrada.");

        var previousProjectId = entity.ProjectId;

        await AllocationCommandHandlerBase.ValidateAsync(
            request, repository, tenantContext, request.Id, cancellationToken);

        AllocationCommandHandlerBase.ApplyToEntity(entity, request);
        entity.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync(cancellationToken);
        await needStatusSync.SyncForProjectAsync(request.ProjectId, cancellationToken);
        if (previousProjectId != request.ProjectId)
            await needStatusSync.SyncForProjectAsync(previousProjectId, cancellationToken);

        var loaded = await repository.GetByIdAsync(entity.Id, cancellationToken)
            ?? throw new InvalidOperationException("Falha ao carregar alocação atualizada.");

        return loaded.ToDto();
    }
}
