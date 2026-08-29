using IAS.Application.AllocationNeeds;
using IAS.Application.Allocations;
using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.Allocations.Commands.DeleteAllocation;

public sealed class DeleteAllocationCommandHandler(
    IAllocationRepository repository,
    IAllocationNeedStatusSync needStatusSync) : IRequestHandler<DeleteAllocationCommand>
{
    public async Task Handle(DeleteAllocationCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Alocação '{request.Id}' não encontrada.");

        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = entity.DeletedAt;

        await repository.SaveChangesAsync(cancellationToken);
        await needStatusSync.SyncForProjectAsync(entity.ProjectId, cancellationToken);
    }
}
