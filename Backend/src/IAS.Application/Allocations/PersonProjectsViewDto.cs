using IAS.Domain.Projects;

namespace IAS.Application.Allocations;

public sealed record PersonProjectsViewDto(
    Guid PersonId,
    string PersonName,
    IReadOnlyList<PersonProjectEntryDto> Projects);

public sealed record PersonProjectEntryDto(
    Guid ProjectId,
    string ProjectName,
    ProjectStatus ProjectStatus,
    IReadOnlyList<AllocationViewItemDto> Allocations);
