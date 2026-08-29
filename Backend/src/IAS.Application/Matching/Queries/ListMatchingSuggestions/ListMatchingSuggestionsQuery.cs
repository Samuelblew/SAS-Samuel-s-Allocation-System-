using IAS.Application.Common.Models;
using IAS.Application.Matching.Commands.RecordMatchingSuggestion;
using IAS.Domain.Matching;
using MediatR;

namespace IAS.Application.Matching.Queries.ListMatchingSuggestions;

public sealed record ListMatchingSuggestionsQuery(
    Guid AllocationNeedId,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<MatchingSuggestionDto>>;
