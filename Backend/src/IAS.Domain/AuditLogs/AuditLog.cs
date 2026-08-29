namespace IAS.Domain.AuditLogs;

public class AuditLog
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public AuditAction Action { get; set; }
    public string? ActorId { get; set; }
    public string? Summary { get; set; }
    public string? ChangesJson { get; set; }
    public DateTime OccurredAt { get; set; }
}
