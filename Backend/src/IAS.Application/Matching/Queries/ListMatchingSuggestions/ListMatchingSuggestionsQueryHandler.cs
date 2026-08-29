using IAS.Application.Common.Exceptions;
using IAS.Application.Common.Models;
using IAS.Application.Matching.Commands.RecordMatchingSuggestion;
using IAS.Domain.Matching;
using MediatR;

namespace IAS.Application.Matching.Queries.ListMatchingSuggestions;

public sealed class ListMatchingSuggestionsQueryHandler(IMatchingSuggestionRepository repository)
    : IRequestHandler<ListMatchingSuggestionsQuery, PagedResult<MatchingSuggestionDto>>
{
    public async Task<PagedResult<MatchingSuggestionDto>> Handle(
        ListMatchingSuggestionsQuery request,
        CancellationToken cancellationToken)
    {
        if (!await repository.AllocationNeedExistsAsync(request.AllocationNeedId, cancellationToken))
            throw new NotFoundException($"Necessidade de alocação '{request.AllocationNeedId}' não encontrada.");

        var (items, total) = await repository.ListByNeedAsync(
            request.AllocationNeedId,
            request.Page,
            request.PageSize,
            cancellationToken);

        var dtos = items.Select(ToDto).ToList();

        return new PagedResult<MatchingSuggestionDto>(
            dtos,
            request.Page,
            request.PageSize,
            total);
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
}
