using IAS.Domain.People;

namespace IAS.Application.Allocations;

public sealed record ProjectPeopleViewDto(
    Guid ProjectId,
    string ProjectName,
    IReadOnlyList<ProjectPersonEntryDto> People);

public sealed record ProjectPersonEntryDto(
    Guid PersonId,
    string PersonName,
    string? JobTitle,
    PersonStatus Status,
    IReadOnlyList<AllocationViewItemDto> Allocations);
