using IAS.Domain.Allocations;

namespace IAS.Application.Allocations;

public sealed record WeeklyOverloadDetail(
    DateOnly WeekStart,
    DateOnly WeekEnd,
    decimal RequestedPercent,
    decimal ExistingPercent,
    decimal TotalPercent,
    IReadOnlyList<AllocationConflictItemDto> ExistingAllocations);
