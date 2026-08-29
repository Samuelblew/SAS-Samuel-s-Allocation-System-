using IAS.Application.AllocationNeeds;
using IAS.Application.Capacity;
using IAS.Domain.AllocationNeeds;
using IAS.Domain.Allocations;
using IAS.Domain.People;
using IAS.Domain.Projects;
using IAS.Domain.Unavailabilities;
using IAS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IAS.Infrastructure.Capacity;

public sealed class CapacityReadRepository(IasDbContext context) : ICapacityReadRepository
{
    private static readonly ProjectStatus[] ActiveProjectStatuses =
    [
        ProjectStatus.Proposal,
        ProjectStatus.Approved,
        ProjectStatus.InProgress,
        ProjectStatus.Paused
    ];

    public Task<Person?> GetPersonAsync(Guid personId, CancellationToken cancellationToken = default) =>
        context.People.FirstOrDefaultAsync(p => p.Id == personId, cancellationToken);

    public Task<Project?> GetProjectAsync(Guid projectId, CancellationToken cancellationToken = default) =>
        context.Projects.FirstOrDefaultAsync(p => p.Id == projectId, cancellationToken);

    public async Task<IReadOnlyList<Allocation>> GetAllocationsForPersonAsync(
        Guid personId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default) =>
        await context.Allocations
            .Where(a => a.PersonId == personId && a.StartDate <= to && a.EndDate >= from)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Unavailability>> GetUnavailabilitiesForPersonAsync(
        Guid personId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default) =>
        await context.Unavailabilities
            .Where(u => u.PersonId == personId && u.StartDate <= to && u.EndDate >= from)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<AllocationNeed>> GetNeedsForProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default) =>
        await context.AllocationNeeds
            .Where(n => n.ProjectId == projectId)
            .OrderBy(n => n.Role)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Allocation>> GetAllocationsForProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default) =>
        await context.Allocations
            .Where(a => a.ProjectId == projectId)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Person>> ListActivePeopleAsync(CancellationToken cancellationToken = default) =>
        await context.People
            .Where(p => p.Status == PersonStatus.Active || p.Status == PersonStatus.Contractor)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Person>> ListActivePeopleWithSkillsAsync(
        CancellationToken cancellationToken = default) =>
        await context.People
            .Include(p => p.Skills)
            .ThenInclude(ps => ps.Skill)
            .Where(p => p.Status == PersonStatus.Active || p.Status == PersonStatus.Contractor)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Allocation>> GetAllocationsInPeriodAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default) =>
        await context.Allocations
            .Include(a => a.Project)
            .Where(a => a.StartDate <= to && a.EndDate >= from)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Unavailability>> GetUnavailabilitiesInPeriodAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default) =>
        await context.Unavailabilities
            .Where(u => u.StartDate <= to && u.EndDate >= from)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<ProjectStaffingSummary>> ListProjectStaffingSummariesAsync(
        CancellationToken cancellationToken = default)
    {
        var projects = await context.Projects
            .Where(p => ActiveProjectStatuses.Contains(p.Status))
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        var needs = await context.AllocationNeeds.ToListAsync(cancellationToken);
        var allocations = await context.Allocations.ToListAsync(cancellationToken);

        var needsByProject = needs.GroupBy(n => n.ProjectId).ToDictionary(g => g.Key, g => g.ToList());
        var allocationsByProject = allocations.GroupBy(a => a.ProjectId).ToDictionary(g => g.Key, g => g.ToList());

        var summaries = new List<ProjectStaffingSummary>();

        foreach (var project in projects)
        {
            var projectNeeds = needsByProject.GetValueOrDefault(project.Id) ?? [];
            if (projectNeeds.Count == 0)
                continue;

            var projectAllocations = allocationsByProject.GetValueOrDefault(project.Id) ?? [];
            var openNeeds = 0;
            var totalGap = 0m;

            foreach (var need in projectNeeds)
            {
                var covered = AllocationNeedStatusCalculator.CalculateCoveredPercent(need, projectAllocations);
                var status = AllocationNeedStatusCalculator.ResolveStatus(covered, need.DedicationPercent);

                if (status != AllocationNeedStatus.Filled)
                {
                    openNeeds++;
                    totalGap += Math.Max(0, need.DedicationPercent - covered);
                }
            }

            if (openNeeds > 0)
            {
                summaries.Add(new ProjectStaffingSummary(
                    project.Id,
                    project.Name,
                    project.Status,
                    openNeeds,
                    totalGap));
            }
        }

        return summaries;
    }

    public async Task<IReadOnlyList<AllocationNeed>> ListAllocationNeedsForActiveProjectsAsync(
        CancellationToken cancellationToken = default)
    {
        var activeProjectIds = await context.Projects
            .Where(p => ActiveProjectStatuses.Contains(p.Status))
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        if (activeProjectIds.Count == 0)
            return [];

        return await context.AllocationNeeds
            .Include(n => n.Project)
            .Where(n => activeProjectIds.Contains(n.ProjectId))
            .OrderBy(n => n.Project.Name)
            .ThenBy(n => n.Role)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Allocation>> GetAllAllocationsAsync(
        CancellationToken cancellationToken = default) =>
        await context.Allocations.ToListAsync(cancellationToken);
}
