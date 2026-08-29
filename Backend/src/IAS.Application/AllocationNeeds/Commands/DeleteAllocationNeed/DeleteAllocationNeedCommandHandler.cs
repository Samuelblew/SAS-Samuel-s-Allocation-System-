using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.AllocationNeeds.Commands.DeleteAllocationNeed;

public sealed class DeleteAllocationNeedCommandHandler(IAllocationNeedRepository repository)
    : IRequestHandler<DeleteAllocationNeedCommand>
{
    public async Task Handle(DeleteAllocationNeedCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Necessidade de alocação '{request.Id}' não encontrada.");

        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = entity.DeletedAt;

        await repository.SaveChangesAsync(cancellationToken);
    }
}
