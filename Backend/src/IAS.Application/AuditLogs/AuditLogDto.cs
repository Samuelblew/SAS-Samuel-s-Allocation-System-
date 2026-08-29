using IAS.Domain.AuditLogs;

namespace IAS.Application.AuditLogs;

public sealed record AuditLogDto(
    Guid Id,
    string EntityType,
    Guid EntityId,
    AuditAction Action,
    string? ActorId,
    string? Summary,
    string? ChangesJson,
    DateTime OccurredAt);
