using IAS.Domain.AllocationNeeds;

namespace IAS.Application.AllocationNeeds;

public interface IAllocationNeedRepository
{
    Task<bool> ProjectExistsAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<bool> AllSkillsExistAsync(IReadOnlyList<Guid> skillIds, CancellationToken cancellationToken = default);
    Task<AllocationNeed?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<AllocationNeed> Items, int Total)> ListAsync(
        int page,
        int pageSize,
        Guid? projectId = null,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AllocationNeed>> ListByProjectAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);
    Task AddAsync(AllocationNeed allocationNeed, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
