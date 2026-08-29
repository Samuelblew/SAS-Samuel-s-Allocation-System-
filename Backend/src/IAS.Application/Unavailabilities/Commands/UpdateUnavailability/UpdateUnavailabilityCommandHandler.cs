using IAS.Application.Common.Exceptions;
using IAS.Application.Unavailabilities;
using MediatR;

namespace IAS.Application.Unavailabilities.Commands.UpdateUnavailability;

public sealed class UpdateUnavailabilityCommandHandler(IUnavailabilityRepository repository)
    : IRequestHandler<UpdateUnavailabilityCommand, UnavailabilityDto>
{
    public async Task<UnavailabilityDto> Handle(
        UpdateUnavailabilityCommand request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Indisponibilidade '{request.Id}' não encontrada.");

        if (entity.PersonId != request.PersonId)
            throw new NotFoundException($"Indisponibilidade '{request.Id}' não pertence à pessoa informada.");

        if (await repository.HasOverlapAsync(
                request.PersonId,
                request.StartDate,
                request.EndDate,
                request.Id,
                cancellationToken))
            throw new ConflictException("Já existe indisponibilidade no período informado.");

        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;
        entity.Type = request.Type;
        entity.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();
        entity.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync(cancellationToken);

        return entity.ToDto();
    }
}
