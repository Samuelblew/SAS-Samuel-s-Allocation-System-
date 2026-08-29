namespace IAS.Api.Dtos;

public sealed record CreateTenantRequest(string Name);

public sealed record TenantResponse(
    Guid Id,
    string Name,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
