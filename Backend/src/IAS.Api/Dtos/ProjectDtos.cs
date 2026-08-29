using IAS.Domain.Projects;

namespace IAS.Api.Dtos;

public sealed record CreateProjectRequest(
    Guid ClientId,
    string Name,
    ProjectStatus Status,
    DateOnly? StartDate,
    DateOnly? EndDate,
    ProjectPriority Priority,
    decimal? Budget,
    decimal? EstimatedRevenue,
    string? ProjectType,
    string? CommercialOwner,
    string? DeliveryOwner);

public sealed record UpdateProjectRequest(
    Guid ClientId,
    string Name,
    ProjectStatus Status,
    DateOnly? StartDate,
    DateOnly? EndDate,
    ProjectPriority Priority,
    decimal? Budget,
    decimal? EstimatedRevenue,
    string? ProjectType,
    string? CommercialOwner,
    string? DeliveryOwner);

public sealed record ProjectResponse(
    Guid Id,
    Guid ClientId,
    string ClientName,
    string Name,
    ProjectStatus Status,
    DateOnly? StartDate,
    DateOnly? EndDate,
    ProjectPriority Priority,
    decimal? Budget,
    decimal? EstimatedRevenue,
    string? ProjectType,
    string? CommercialOwner,
    string? DeliveryOwner,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record ProjectListItemResponse(
    Guid Id,
    Guid ClientId,
    string ClientName,
    string Name,
    ProjectStatus Status,
    ProjectPriority Priority,
    DateOnly? StartDate,
    DateOnly? EndDate,
    DateTime CreatedAt);

public sealed record PagedProjectsResponse(
    IReadOnlyList<ProjectListItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
