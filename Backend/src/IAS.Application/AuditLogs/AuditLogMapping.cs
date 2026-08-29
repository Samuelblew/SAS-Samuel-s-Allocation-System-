using IAS.Domain.AuditLogs;

namespace IAS.Application.AuditLogs;

internal static class AuditLogMapping
{
    public static AuditLogDto ToDto(this AuditLog entity) =>
        new(
            entity.Id,
            entity.EntityType,
            entity.EntityId,
            entity.Action,
            entity.ActorId,
            entity.Summary,
            entity.ChangesJson,
            entity.OccurredAt);
}
