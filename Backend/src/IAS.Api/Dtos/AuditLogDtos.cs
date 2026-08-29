using IAS.Domain.AuditLogs;

namespace IAS.Api.Dtos;

public sealed record AuditLogResponse(
    Guid Id,
    string EntityType,
    Guid EntityId,
    AuditAction Action,
    string? ActorId,
    string? Summary,
    string? ChangesJson,
    DateTime OccurredAt);

public sealed record PagedAuditLogsResponse(
    IReadOnlyList<AuditLogResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
