namespace IAS.Application.Clients;

public sealed record ClientDto(
    Guid Id,
    string Name,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record ClientListItemDto(
    Guid Id,
    string Name,
    int ProjectCount,
    DateTime CreatedAt);
