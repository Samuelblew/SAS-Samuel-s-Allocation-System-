using IAS.Application.Common.Interfaces;

namespace IAS.Infrastructure.Tenancy;

public sealed class TenantContext : ITenantContext
{
    public const string TenantIdItemKey = "IAS.TenantId";

    public Guid TenantId { get; private set; }
    public bool IsResolved { get; private set; }

    public void SetTenant(Guid tenantId)
    {
        TenantId = tenantId;
        IsResolved = true;
    }
}
