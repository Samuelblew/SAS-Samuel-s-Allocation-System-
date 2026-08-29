using IAS.Domain.AllocationNeeds;
using IAS.Domain.Allocations;
using IAS.Domain.People;
using IAS.Domain.Projects;
using IAS.Domain.Unavailabilities;

namespace IAS.Application.Capacity;

public interface ICapacityReadRepository
{
    Task<Person?> GetPersonAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<Project?> GetProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Allocation>> GetAllocationsForPersonAsync(
        Guid personId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Unavailability>> GetUnavailabilitiesForPersonAsync(
        Guid personId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AllocationNeed>> GetNeedsForProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Allocation>> GetAllocationsForProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Person>> ListActivePeopleAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Person>> ListActivePeopleWithSkillsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Allocation>> GetAllocationsInPeriodAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Unavailability>> GetUnavailabilitiesInPeriodAsync(
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProjectStaffingSummary>> ListProjectStaffingSummariesAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AllocationNeed>> ListAllocationNeedsForActiveProjectsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Allocation>> GetAllAllocationsAsync(
        CancellationToken cancellationToken = default);
}

public sealed record ProjectStaffingSummary(
    Guid ProjectId,
    string ProjectName,
    ProjectStatus Status,
    int OpenNeedsCount,
    decimal TotalGapPercent);
