using IAS.Domain.Unavailabilities;

namespace IAS.Application.Unavailabilities;

public sealed record UnavailabilityDto(
    Guid Id,
    Guid PersonId,
    DateOnly StartDate,
    DateOnly EndDate,
    UnavailabilityType Type,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
