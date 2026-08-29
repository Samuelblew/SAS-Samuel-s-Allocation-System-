using IAS.Domain.Matching;

namespace IAS.Application.Matching;

public interface IMatchingSuggestionRepository
{
    Task<bool> AllocationNeedExistsAsync(Guid allocationNeedId, CancellationToken cancellationToken = default);
    Task<bool> PersonExistsAsync(Guid personId, CancellationToken cancellationToken = default);
    Task AddAsync(MatchingSuggestion suggestion, CancellationToken cancellationToken = default);
    Task<MatchingSuggestion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<MatchingSuggestion> Items, int Total)> ListByNeedAsync(
        Guid allocationNeedId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
