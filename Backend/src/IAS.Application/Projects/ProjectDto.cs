using IAS.Domain.Projects;

namespace IAS.Application.Projects;

public sealed record ProjectDto(
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

public sealed record ProjectListItemDto(
    Guid Id,
    Guid ClientId,
    string ClientName,
    string Name,
    ProjectStatus Status,
    ProjectPriority Priority,
    DateOnly? StartDate,
    DateOnly? EndDate,
    DateTime CreatedAt);
