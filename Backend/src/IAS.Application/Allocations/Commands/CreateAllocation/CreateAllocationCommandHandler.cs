using IAS.Application.AllocationNeeds;
using IAS.Application.Allocations;
using IAS.Application.Common.Interfaces;
using IAS.Domain.Allocations;
using MediatR;

namespace IAS.Application.Allocations.Commands.CreateAllocation;

public sealed class CreateAllocationCommandHandler(
    IAllocationRepository repository,
    IAllocationNeedStatusSync needStatusSync,
    ITenantContext tenantContext) : IRequestHandler<CreateAllocationCommand, AllocationDto>
{
    public async Task<AllocationDto> Handle(
        CreateAllocationCommand request,
        CancellationToken cancellationToken)
    {
        await AllocationCommandHandlerBase.ValidateAsync(
            request, repository, tenantContext, null, cancellationToken);

        var entity = new Allocation
        {
            Id = Guid.NewGuid(),
            TenantId = tenantContext.TenantId,
            CreatedAt = DateTime.UtcNow
        };

        AllocationCommandHandlerBase.ApplyToEntity(entity, request);

        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);
        await needStatusSync.SyncForProjectAsync(request.ProjectId, cancellationToken);

        var loaded = await repository.GetByIdAsync(entity.Id, cancellationToken)
            ?? throw new InvalidOperationException("Falha ao carregar alocação criada.");

        return loaded.ToDto();
    }
}
