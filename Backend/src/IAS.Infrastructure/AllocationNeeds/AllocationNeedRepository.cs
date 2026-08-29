using IAS.Application.AllocationNeeds;
using IAS.Domain.AllocationNeeds;
using IAS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IAS.Infrastructure.AllocationNeeds;

public sealed class AllocationNeedRepository(IasDbContext context) : IAllocationNeedRepository
{
    public Task<bool> ProjectExistsAsync(Guid projectId, CancellationToken cancellationToken = default) =>
        context.Projects.AnyAsync(p => p.Id == projectId, cancellationToken);

    public async Task<bool> AllSkillsExistAsync(
        IReadOnlyList<Guid> skillIds,
        CancellationToken cancellationToken = default)
    {
        if (skillIds.Count == 0)
            return true;

        var existing = await context.Skills
            .Where(s => skillIds.Contains(s.Id))
            .CountAsync(cancellationToken);

        return existing == skillIds.Count;
    }

    public Task<AllocationNeed?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.AllocationNeeds
            .Include(n => n.Project)
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<AllocationNeed> Items, int Total)> ListAsync(
        int page,
        int pageSize,
        Guid? projectId = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.AllocationNeeds.Include(n => n.Project).AsQueryable();

        if (projectId.HasValue)
            query = query.Where(n => n.ProjectId == projectId.Value);

        query = query.OrderByDescending(n => n.CreatedAt);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<IReadOnlyList<AllocationNeed>> ListByProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default) =>
        await context.AllocationNeeds
            .Include(n => n.Project)
            .Where(n => n.ProjectId == projectId)
            .OrderBy(n => n.Role)
            .ThenByDescending(n => n.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(AllocationNeed allocationNeed, CancellationToken cancellationToken = default) =>
        await context.AllocationNeeds.AddAsync(allocationNeed, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
