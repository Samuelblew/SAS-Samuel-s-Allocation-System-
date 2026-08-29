namespace IAS.Api.Dtos;

public sealed record CreateClientRequest(string Name, string? Notes);

public sealed record UpdateClientRequest(string Name, string? Notes);

public sealed record ClientResponse(
    Guid Id,
    string Name,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record ClientListItemResponse(
    Guid Id,
    string Name,
    int ProjectCount,
    DateTime CreatedAt);

public sealed record PagedClientsResponse(
    IReadOnlyList<ClientListItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
