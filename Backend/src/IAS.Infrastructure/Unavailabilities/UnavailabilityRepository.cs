using IAS.Application.Unavailabilities;
using IAS.Domain.Unavailabilities;
using IAS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IAS.Infrastructure.Unavailabilities;

public sealed class UnavailabilityRepository(IasDbContext context) : IUnavailabilityRepository
{
    public Task<bool> PersonExistsAsync(Guid personId, CancellationToken cancellationToken = default) =>
        context.People.AnyAsync(p => p.Id == personId, cancellationToken);

    public Task<Unavailability?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        context.Unavailabilities.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<bool> HasOverlapAsync(
        Guid personId,
        DateOnly startDate,
        DateOnly endDate,
        Guid? excludeId = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.Unavailabilities.Where(u =>
            u.PersonId == personId
            && u.StartDate <= endDate
            && u.EndDate >= startDate);

        if (excludeId.HasValue)
            query = query.Where(u => u.Id != excludeId.Value);

        return query.AnyAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Unavailability> Items, int Total)> ListByPersonAsync(
        Guid personId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.Unavailabilities
            .Where(u => u.PersonId == personId)
            .OrderBy(u => u.StartDate);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task AddAsync(Unavailability unavailability, CancellationToken cancellationToken = default) =>
        await context.Unavailabilities.AddAsync(unavailability, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
