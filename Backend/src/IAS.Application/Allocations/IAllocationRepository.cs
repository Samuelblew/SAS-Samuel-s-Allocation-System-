using IAS.Domain.Allocations;
using IAS.Domain.People;

namespace IAS.Application.Allocations;

public interface IAllocationRepository
{
    Task<Person?> GetPersonAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<bool> ProjectExistsAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<Allocation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Allocation>> GetOverlappingForPersonAsync(
        Guid personId,
        DateOnly start,
        DateOnly end,
        Guid? excludeAllocationId = null,
        CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Allocation> Items, int Total)> ListAsync(
        int page,
        int pageSize,
        Guid? personId = null,
        Guid? projectId = null,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Allocation>> GetByProjectIdAsync(
        Guid projectId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Allocation>> GetByPersonIdAsync(
        Guid personId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Allocation>> GetActiveForConflictScanAsync(
        Guid? personId = null,
        Guid? projectId = null,
        DateOnly? from = null,
        DateOnly? to = null,
        CancellationToken cancellationToken = default);
    Task AddAsync(Allocation allocation, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
