namespace IAS.Domain.Common;

/// <summary>
/// Base para entidades multi-tenant (tenant_id em todas as tabelas de negócio).
/// </summary>
public abstract class TenantEntity
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
