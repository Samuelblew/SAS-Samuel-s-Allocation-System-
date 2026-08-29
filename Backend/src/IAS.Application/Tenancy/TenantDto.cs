namespace IAS.Application.Tenancy;

public sealed record TenantDto(
    Guid Id,
    string Name,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
