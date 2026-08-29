using IAS.Domain.Unavailabilities;

namespace IAS.Api.Dtos;

public sealed record CreateUnavailabilityRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    UnavailabilityType Type,
    string? Notes);

public sealed record UpdateUnavailabilityRequest(
    DateOnly StartDate,
    DateOnly EndDate,
    UnavailabilityType Type,
    string? Notes);

public sealed record UnavailabilityResponse(
    Guid Id,
    Guid PersonId,
    DateOnly StartDate,
    DateOnly EndDate,
    UnavailabilityType Type,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record PagedUnavailabilitiesResponse(
    IReadOnlyList<UnavailabilityResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
