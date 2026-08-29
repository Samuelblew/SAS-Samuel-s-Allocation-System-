using IAS.Domain.Allocations;

namespace IAS.Application.Allocations;

internal static class AllocationMapping
{
    public static AllocationDto ToDto(this Allocation entity) =>
        new(
            entity.Id,
            entity.PersonId,
            entity.Person.Name,
            entity.ProjectId,
            entity.Project.Name,
            entity.Role,
            entity.DedicationPercent,
            entity.StartDate,
            entity.EndDate,
            entity.Status,
            entity.Notes,
            entity.CreatedAt,
            entity.UpdatedAt);
}
