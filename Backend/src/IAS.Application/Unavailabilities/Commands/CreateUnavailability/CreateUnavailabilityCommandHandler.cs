using IAS.Application.Common.Exceptions;
using IAS.Application.Common.Interfaces;
using IAS.Application.Unavailabilities;
using IAS.Domain.Unavailabilities;
using MediatR;

namespace IAS.Application.Unavailabilities.Commands.CreateUnavailability;

public sealed class CreateUnavailabilityCommandHandler(
    IUnavailabilityRepository repository,
    ITenantContext tenantContext) : IRequestHandler<CreateUnavailabilityCommand, UnavailabilityDto>
{
    public async Task<UnavailabilityDto> Handle(
        CreateUnavailabilityCommand request,
        CancellationToken cancellationToken)
    {
        if (!tenantContext.IsResolved)
            throw new InvalidOperationException("Tenant não resolvido.");

        if (!await repository.PersonExistsAsync(request.PersonId, cancellationToken))
            throw new NotFoundException($"Pessoa '{request.PersonId}' não encontrada.");

        if (await repository.HasOverlapAsync(
                request.PersonId,
                request.StartDate,
                request.EndDate,
                cancellationToken: cancellationToken))
            throw new ConflictException("Já existe indisponibilidade no período informado.");

        var entity = new Unavailability
        {
            Id = Guid.NewGuid(),
            TenantId = tenantContext.TenantId,
            PersonId = request.PersonId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Type = request.Type,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return entity.ToDto();
    }
}
