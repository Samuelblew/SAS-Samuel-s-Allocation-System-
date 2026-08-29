using IAS.Application.Projects;
using IAS.Domain.Projects;
using MediatR;

namespace IAS.Application.Projects.Commands.UpdateProject;

public sealed record UpdateProjectCommand(
    Guid Id,
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
    string? DeliveryOwner) : IRequest<ProjectDto>;
