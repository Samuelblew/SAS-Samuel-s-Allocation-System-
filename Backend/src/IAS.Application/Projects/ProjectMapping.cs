using IAS.Domain.Projects;

namespace IAS.Application.Projects;

internal static class ProjectMapping
{
    public static ProjectDto ToDto(this Project project) =>
        new(
            project.Id,
            project.ClientId,
            project.Client.Name,
            project.Name,
            project.Status,
            project.StartDate,
            project.EndDate,
            project.Priority,
            project.Budget,
            project.EstimatedRevenue,
            project.ProjectType,
            project.CommercialOwner,
            project.DeliveryOwner,
            project.CreatedAt,
            project.UpdatedAt);

    public static ProjectListItemDto ToListItemDto(this Project project) =>
        new(
            project.Id,
            project.ClientId,
            project.Client.Name,
            project.Name,
            project.Status,
            project.Priority,
            project.StartDate,
            project.EndDate,
            project.CreatedAt);
}
