using IAS.Domain.Allocations;

namespace IAS.Application.Allocations;

public sealed record AllocationDto(
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
