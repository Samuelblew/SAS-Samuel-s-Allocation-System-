using IAS.Domain.Tenancy;

namespace IAS.Application.Tenancy;

internal static class TenantMapping
{
    public static TenantDto ToDto(this Tenant tenant) =>
        new(tenant.Id, tenant.Name, tenant.IsActive, tenant.CreatedAt, tenant.UpdatedAt);
}
