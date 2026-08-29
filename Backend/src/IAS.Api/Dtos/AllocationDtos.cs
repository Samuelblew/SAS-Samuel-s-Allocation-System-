using IAS.Domain.Allocations;

namespace IAS.Api.Dtos;

public sealed record CreateAllocationRequest(
    Guid PersonId,
    Guid ProjectId,
    string Role,
    decimal DedicationPercent,
    DateOnly StartDate,
    DateOnly EndDate,
    AllocationStatus Status,
    string? Notes);

public sealed record UpdateAllocationRequest(
    Guid PersonId,
    Guid ProjectId,
    string Role,
    decimal DedicationPercent,
    DateOnly StartDate,
    DateOnly EndDate,
    AllocationStatus Status,
    string? Notes);

public sealed record AllocationResponse(
    Guid Id,
    Guid PersonId,
    string PersonName,
    Guid ProjectId,
    string ProjectName,
    string Role,
    decimal DedicationPercent,
    DateOnly StartDate,
    DateOnly EndDate,
    AllocationStatus Status,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record AllocationListItemResponse(
    Guid Id,
    Guid PersonId,
    string PersonName,
    Guid ProjectId,
    string ProjectName,
    string Role,
    decimal DedicationPercent,
    AllocationStatus Status,
    DateOnly StartDate,
    DateOnly EndDate,
    DateTime CreatedAt);

public sealed record PagedAllocationsResponse(
    IReadOnlyList<AllocationListItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
