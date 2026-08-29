using IAS.Domain.Matching;
using MediatR;

namespace IAS.Application.Matching.Commands.RecordMatchingSuggestion;

public sealed record RecordMatchingSuggestionCommand(
    Guid AllocationNeedId,
    Guid PersonId,
    MatchingSuggestionDecision Decision,
    decimal Score,
    string? Notes) : IRequest<MatchingSuggestionDto>;

public sealed record MatchingSuggestionDto(
    Guid Id,
    Guid AllocationNeedId,
    string ProjectName,
    string NeedRole,
    Guid PersonId,
    string PersonName,
    MatchingSuggestionDecision Decision,
    decimal Score,
    string? Notes,
    Guid? DecidedByUserId,
    DateTime CreatedAt);
