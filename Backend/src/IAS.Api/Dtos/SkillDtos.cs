namespace IAS.Api.Dtos;

public sealed record CreateSkillRequest(string Name, string? Category);

public sealed record UpdateSkillRequest(string Name, string? Category);

public sealed record SkillResponse(
    Guid Id,
    string Name,
    string? Category,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record PagedSkillsResponse(
    IReadOnlyList<SkillResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
