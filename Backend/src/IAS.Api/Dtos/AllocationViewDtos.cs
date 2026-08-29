using IAS.Domain.Allocations;
using IAS.Domain.People;
using IAS.Domain.Projects;

namespace IAS.Api.Dtos;

public sealed record AllocationViewItemResponse(
    Guid Id,
    string Role,
    decimal DedicationPercent,
    DateOnly StartDate,
    DateOnly EndDate,
    AllocationStatus Status,
    string? Notes);

public sealed record ProjectPersonEntryResponse(
    Guid PersonId,
    string PersonName,
    string? JobTitle,
    PersonStatus Status,
    IReadOnlyList<AllocationViewItemResponse> Allocations);

public sealed record ProjectPeopleViewResponse(
    Guid ProjectId,
    string ProjectName,
    IReadOnlyList<ProjectPersonEntryResponse> People);

public sealed record PersonProjectEntryResponse(
    Guid ProjectId,
    string ProjectName,
    ProjectStatus ProjectStatus,
    IReadOnlyList<AllocationViewItemResponse> Allocations);

public sealed record PersonProjectsViewResponse(
    Guid PersonId,
    string PersonName,
    IReadOnlyList<PersonProjectEntryResponse> Projects);

public sealed record AllocationConflictItemResponse(
    Guid AllocationId,
    Guid ProjectId,
    string ProjectName,
    decimal DedicationPercent,
    DateOnly StartDate,
    DateOnly EndDate,
    AllocationStatus Status);

public sealed record AllocationConflictResponse(
    Guid PersonId,
    string PersonName,
    DateOnly WeekStart,
    DateOnly WeekEnd,
    decimal TotalDedicationPercent,
    IReadOnlyList<AllocationConflictItemResponse> Allocations);

public sealed record AllocationConflictsListResponse(
    IReadOnlyList<AllocationConflictResponse> Items);
