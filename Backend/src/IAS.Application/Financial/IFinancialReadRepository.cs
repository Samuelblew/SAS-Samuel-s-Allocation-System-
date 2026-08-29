using IAS.Domain.Allocations;
using IAS.Domain.Projects;

namespace IAS.Application.Financial;

public interface IFinancialReadRepository
{
    Task<Project?> GetProjectWithClientAsync(Guid projectId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Allocation>> GetProjectAllocationsWithPeopleAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Project>> ListActiveProjectsWithClientAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Allocation>> GetAllocationsForProjectsAsync(
        IReadOnlyList<Guid> projectIds,
        CancellationToken cancellationToken = default);
}
