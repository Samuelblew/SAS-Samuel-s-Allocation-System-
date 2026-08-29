using IAS.Domain.Identity;

namespace IAS.Api.Dtos;

public sealed record CreateUserRequest(
    string Email,
    string DisplayName,
    UserRole Role);

public sealed record UpdateUserRequest(
    string Email,
    string DisplayName,
    UserRole Role,
    bool IsActive);

public sealed record UserResponse(
    Guid Id,
    string Email,
    string DisplayName,
    UserRole Role,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record PagedUsersResponse(
    IReadOnlyList<UserResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
