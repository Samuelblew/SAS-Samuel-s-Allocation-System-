using IAS.Domain.Projects;

namespace IAS.Application.Projects;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ClientExistsAsync(Guid clientId, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Project> Items, int Total)> ListAsync(
        int page,
        int pageSize,
        Guid? clientId = null,
        CancellationToken cancellationToken = default);
    Task AddAsync(Project project, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
