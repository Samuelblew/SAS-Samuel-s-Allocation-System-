using IAS.Domain.Allocations;

namespace IAS.Application.Allocations;

public sealed record AllocationViewItemDto(
    Guid Id,
    string Role,
    decimal DedicationPercent,
    DateOnly StartDate,
    DateOnly EndDate,
    AllocationStatus Status,
    string? Notes);
