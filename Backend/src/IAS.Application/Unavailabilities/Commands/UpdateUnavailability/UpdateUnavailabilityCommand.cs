using IAS.Application.Unavailabilities;
using IAS.Domain.Unavailabilities;
using MediatR;

namespace IAS.Application.Unavailabilities.Commands.UpdateUnavailability;

public sealed record UpdateUnavailabilityCommand(
    Guid PersonId,
    Guid Id,
    DateOnly StartDate,
    DateOnly EndDate,
    UnavailabilityType Type,
    string? Notes) : IRequest<UnavailabilityDto>;
