using MediatR;

namespace IAS.Application.Capacity.Queries.GetPersonAvailability;

public sealed record GetPersonAvailabilityQuery(
    Guid PersonId,
    DateOnly From,
    DateOnly To) : IRequest<PersonAvailabilityDto>;

public sealed record PersonAvailabilityDto(
    Guid PersonId,
    string PersonName,
    decimal WeeklyCapacityHours,
    DateOnly From,
    DateOnly To,
    IReadOnlyList<WeekAvailabilityDto> Weeks);

public sealed record WeekAvailabilityDto(
    DateOnly WeekStart,
    DateOnly WeekEnd,
    decimal AllocatedPercent,
    decimal AvailablePercent,
    decimal WeeklyCapacityHours,
    decimal AllocatedHours,
    decimal AvailableHours,
    bool IsUnavailable);
