using IAS.Application.Common.Exceptions;
using IAS.Application.Common.Interfaces;
using IAS.Domain.Matching;
using MediatR;

namespace IAS.Application.Matching.Commands.RecordMatchingSuggestion;

public sealed class RecordMatchingSuggestionCommandHandler(
    IMatchingSuggestionRepository repository,
    ITenantContext tenantContext,
    IAuditActorContext auditActorContext) : IRequestHandler<RecordMatchingSuggestionCommand, MatchingSuggestionDto>
{
    public async Task<MatchingSuggestionDto> Handle(
        RecordMatchingSuggestionCommand request,
        CancellationToken cancellationToken)
    {
        if (!await repository.AllocationNeedExistsAsync(request.AllocationNeedId, cancellationToken))
            throw new NotFoundException($"Necessidade de alocação '{request.AllocationNeedId}' não encontrada.");

        if (!await repository.PersonExistsAsync(request.PersonId, cancellationToken))
            throw new NotFoundException($"Pessoa '{request.PersonId}' não encontrada.");

        var entity = new MatchingSuggestion
        {
            Id = Guid.NewGuid(),
            TenantId = tenantContext.TenantId,
            AllocationNeedId = request.AllocationNeedId,
            PersonId = request.PersonId,
            Decision = request.Decision,
            Score = Math.Round(request.Score, 2),
            Notes = TrimOrNull(request.Notes),
            DecidedByUserId = ParseActorId(auditActorContext.ActorId),
            CreatedAt = DateTime.UtcNow
        };

        await repository.AddAsync(entity, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        var loaded = await repository.GetByIdAsync(entity.Id, cancellationToken)
            ?? throw new InvalidOperationException("Falha ao carregar sugestão registrada.");

        return ToDto(loaded);
    }

    private static MatchingSuggestionDto ToDto(MatchingSuggestion s) =>
        new(
            s.Id,
            s.AllocationNeedId,
            s.AllocationNeed.Project.Name,
            s.AllocationNeed.Role,
            s.PersonId,
            s.Person.Name,
            s.Decision,
            s.Score,
            s.Notes,
            s.DecidedByUserId,
            s.CreatedAt);

    private static string? TrimOrNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static Guid? ParseActorId(string? actorId) =>
        Guid.TryParse(actorId, out var id) ? id : null;
}
