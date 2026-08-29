using IAS.Application.Allocations;
using IAS.Domain.Allocations;
using IAS.Domain.People;
using IAS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IAS.Infrastructure.Allocations;

public sealed class AllocationRepository(IasDbContext context) : IAllocationRepository
{
    public Task<Person?> GetPersonAsync(Guid personId, CancellationToken cancellationToken = default) =>
        context.People.FirstOrDefaultAsync(p => p.Id == personId, cancellationToken);

    public Task<bool> ProjectExistsAsync(Guid projectId, CancellationToken cancellationToken = default) =>
        context.Projects.AnyAsync(p => p.Id == projectId, cancellationToken);

    public Task<Allocation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Allocations
            .Include(a => a.Person)
            .Include(a => a.Project)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Allocation>> GetOverlappingForPersonAsync(
        Guid personId,
        DateOnly start,
        DateOnly end,
        Guid? excludeAllocationId = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Allocations
            .Include(a => a.Project)
            .Where(a => a.PersonId == personId)
            .Where(a => a.StartDate <= end && a.EndDate >= start);

        if (excludeAllocationId.HasValue)
            query = query.Where(a => a.Id != excludeAllocationId.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Allocation>> GetByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default) =>
        await context.Allocations
            .Include(a => a.Person)
            .Include(a => a.Project)
            .Where(a => a.ProjectId == projectId)
            .OrderBy(a => a.Person.Name)
            .ThenByDescending(a => a.StartDate)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Allocation>> GetByPersonIdAsync(
        Guid personId,
        CancellationToken cancellationToken = default) =>
        await context.Allocations
            .Include(a => a.Person)
            .Include(a => a.Project)
            .Where(a => a.PersonId == personId)
            .OrderBy(a => a.Project.Name)
            .ThenByDescending(a => a.StartDate)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Allocation>> GetActiveForConflictScanAsync(
        Guid? personId = null,
        Guid? projectId = null,
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Allocations
            .Include(a => a.Person)
            .Include(a => a.Project)
            .Where(a => a.Status != AllocationStatus.Closed);

        if (personId.HasValue)
            query = query.Where(a => a.PersonId == personId.Value);

        if (projectId.HasValue)
            query = query.Where(a => a.ProjectId == projectId.Value);

        if (from.HasValue)
            query = query.Where(a => a.EndDate >= from.Value);

        if (to.HasValue)
            query = query.Where(a => a.StartDate <= to.Value);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Allocation> Items, int Total)> ListAsync(
        int page,
        int pageSize,
        Guid? personId = null,
        Guid? projectId = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Allocations
            .Include(a => a.Person)
            .Include(a => a.Project)
            .AsQueryable();

        if (personId.HasValue)
            query = query.Where(a => a.PersonId == personId.Value);

        if (projectId.HasValue)
            query = query.Where(a => a.ProjectId == projectId.Value);

        query = query.OrderByDescending(a => a.CreatedAt);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task AddAsync(Allocation allocation, CancellationToken cancellationToken = default) =>
        await context.Allocations.AddAsync(allocation, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
