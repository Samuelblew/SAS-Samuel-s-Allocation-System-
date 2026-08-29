using IAS.Domain.AuditLogs;

namespace IAS.Application.AuditLogs;

public interface IAuditLogRepository
{
    Task<(IReadOnlyList<AuditLog> Items, int Total)> ListAsync(
        int page,
        int pageSize,
        string? entityType = null,
        Guid? entityId = null,
        AuditAction? action = null,
        DateTime? from = null,
        DateTime? to = null,
        CancellationToken cancellationToken = default);
}
