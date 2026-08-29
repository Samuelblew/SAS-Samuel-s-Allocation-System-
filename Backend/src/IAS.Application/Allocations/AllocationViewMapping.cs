using IAS.Domain.Allocations;

namespace IAS.Application.Allocations;

internal static class AllocationViewMapping
{
    public static AllocationViewItemDto ToViewItem(this Allocation entity) =>
        new(
            entity.Id,
            entity.Role,
            entity.DedicationPercent,
            entity.StartDate,
            entity.EndDate,
            entity.Status,
            entity.Notes);

    public static ProjectPeopleViewDto ToProjectPeopleView(
        this IReadOnlyList<Allocation> allocations,
        Guid projectId,
        string projectName) =>
        new(
            projectId,
            projectName,
            allocations
                .GroupBy(a => a.PersonId)
                .Select(g =>
                {
                    var first = g.First();
                    return new ProjectPersonEntryDto(
                        first.PersonId,
                        first.Person.Name,
                        first.Person.JobTitle,
                        first.Person.Status,
                        g.Select(a => a.ToViewItem()).ToList());
                })
                .OrderBy(p => p.PersonName)
                .ToList());

    public static PersonProjectsViewDto ToPersonProjectsView(
        this IReadOnlyList<Allocation> allocations,
        Guid personId,
        string personName) =>
        new(
            personId,
            personName,
            allocations
                .GroupBy(a => a.ProjectId)
                .Select(g =>
                {
                    var first = g.First();
                    return new PersonProjectEntryDto(
                        first.ProjectId,
                        first.Project.Name,
                        first.Project.Status,
                        g.Select(a => a.ToViewItem()).ToList());
                })
                .OrderBy(p => p.ProjectName)
                .ToList());
}
