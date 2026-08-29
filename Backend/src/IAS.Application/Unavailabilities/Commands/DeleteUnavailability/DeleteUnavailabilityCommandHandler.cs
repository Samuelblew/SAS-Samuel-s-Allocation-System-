using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.Unavailabilities.Commands.DeleteUnavailability;

public sealed class DeleteUnavailabilityCommandHandler(IUnavailabilityRepository repository)
    : IRequestHandler<DeleteUnavailabilityCommand>
{
    public async Task Handle(DeleteUnavailabilityCommand request, CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Indisponibilidade '{request.Id}' não encontrada.");

        if (entity.PersonId != request.PersonId)
            throw new NotFoundException($"Indisponibilidade '{request.Id}' não pertence à pessoa informada.");

        entity.DeletedAt = DateTime.UtcNow;
        entity.UpdatedAt = entity.DeletedAt;

        await repository.SaveChangesAsync(cancellationToken);
    }
}
