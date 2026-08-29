using IAS.Domain.Matching;

namespace IAS.Api.Dtos;

public sealed record RecordMatchingSuggestionRequest(
    Guid PersonId,
    MatchingSuggestionDecision Decision,
    decimal Score,
    string? Notes);

public sealed record MatchingSuggestionResponse(
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

public sealed record PagedMatchingSuggestionsResponse(
    IReadOnlyList<MatchingSuggestionResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
