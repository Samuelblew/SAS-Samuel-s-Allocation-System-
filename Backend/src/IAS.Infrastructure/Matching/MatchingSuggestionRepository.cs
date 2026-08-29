using IAS.Application.Matching;
using IAS.Domain.Matching;
using IAS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IAS.Infrastructure.Matching;

public sealed class MatchingSuggestionRepository(IasDbContext context) : IMatchingSuggestionRepository
{
    public Task<bool> AllocationNeedExistsAsync(Guid allocationNeedId, CancellationToken cancellationToken = default) =>
        context.AllocationNeeds.AnyAsync(n => n.Id == allocationNeedId, cancellationToken);

    public Task<bool> PersonExistsAsync(Guid personId, CancellationToken cancellationToken = default) =>
        context.People.AnyAsync(p => p.Id == personId, cancellationToken);

    public async Task AddAsync(MatchingSuggestion suggestion, CancellationToken cancellationToken = default) =>
        await context.MatchingSuggestions.AddAsync(suggestion, cancellationToken);

    public Task<MatchingSuggestion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.MatchingSuggestions
            .Include(s => s.Person)
            .Include(s => s.AllocationNeed)
            .ThenInclude(n => n.Project)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<MatchingSuggestion> Items, int Total)> ListByNeedAsync(
        Guid allocationNeedId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.MatchingSuggestions
            .Include(s => s.Person)
            .Include(s => s.AllocationNeed)
            .ThenInclude(n => n.Project)
            .Where(s => s.AllocationNeedId == allocationNeedId)
            .OrderByDescending(s => s.CreatedAt);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
