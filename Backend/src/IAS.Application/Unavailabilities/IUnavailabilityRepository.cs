using IAS.Domain.Unavailabilities;

namespace IAS.Application.Unavailabilities;

public interface IUnavailabilityRepository
{
    Task<bool> PersonExistsAsync(Guid personId, CancellationToken cancellationToken = default);
    Task<Unavailability?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> HasOverlapAsync(
        Guid personId,
        DateOnly startDate,
        DateOnly endDate,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Unavailability> Items, int Total)> ListByPersonAsync(
        Guid personId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
    Task AddAsync(Unavailability unavailability, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
