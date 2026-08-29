using IAS.Application.AuditLogs;
using IAS.Domain.AuditLogs;
using IAS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IAS.Infrastructure.AuditLogs;

public sealed class AuditLogRepository(IasDbContext context) : IAuditLogRepository
{
    public async Task<(IReadOnlyList<AuditLog> Items, int Total)> ListAsync(
        int page,
        int pageSize,
        string? entityType = null,
        Guid? entityId = null,
        AuditAction? action = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var query = context.AuditLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(entityType))
            query = query.Where(x => x.EntityType == entityType);

        if (entityId.HasValue)
            query = query.Where(x => x.EntityId == entityId.Value);

        if (action.HasValue)
            query = query.Where(x => x.Action == action.Value);

        if (from.HasValue)
            query = query.Where(x => x.OccurredAt >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.OccurredAt <= to.Value);

        query = query.OrderByDescending(x => x.OccurredAt);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}
