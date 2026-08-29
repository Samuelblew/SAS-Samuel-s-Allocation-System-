using IAS.Application.Financial;
using IAS.Domain.Allocations;
using IAS.Domain.Projects;
using IAS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IAS.Infrastructure.Financial;

public sealed class FinancialReadRepository(IasDbContext context) : IFinancialReadRepository
{
    private static readonly ProjectStatus[] ActiveProjectStatuses =
    [
        ProjectStatus.Proposal,
        ProjectStatus.Approved,
        ProjectStatus.InProgress,
        ProjectStatus.Paused
    ];

    public Task<Project?> GetProjectWithClientAsync(Guid projectId, CancellationToken cancellationToken = default) =>
        context.Projects
            .Include(p => p.Client)
            .FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);

    public async Task<IReadOnlyList<Allocation>> GetProjectAllocationsWithPeopleAsync(
        Guid projectId,
        CancellationToken cancellationToken = default) =>
        await context.Allocations
            .Include(a => a.Person)
            .Where(a => a.ProjectId == projectId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Project>> ListActiveProjectsWithClientAsync(
        CancellationToken cancellationToken = default) =>
        await context.Projects
            .Include(p => p.Client)
            .Where(p => ActiveProjectStatuses.Contains(p.Status))
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Allocation>> GetAllocationsForProjectsAsync(
        IReadOnlyList<Guid> projectIds,
        CancellationToken cancellationToken = default)
    {
        if (projectIds.Count == 0)
            return [];

        return await context.Allocations
            .Include(a => a.Person)
            .Where(a => projectIds.Contains(a.ProjectId))
            .ToListAsync(cancellationToken);
    }
}
