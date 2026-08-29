namespace IAS.Application.Allocations;

public sealed record AllocationConflictDto(
    Guid PersonId,
    string PersonName,
    DateOnly WeekStart,
    DateOnly WeekEnd,
    decimal TotalDedicationPercent,
    IReadOnlyList<AllocationConflictItemDto> Allocations);

public sealed record AllocationConflictItemDto(
    Guid AllocationId,
    Guid ProjectId,
    string ProjectName,
    decimal DedicationPercent,
    DateOnly StartDate,
    DateOnly EndDate,
    Domain.Allocations.AllocationStatus Status);
