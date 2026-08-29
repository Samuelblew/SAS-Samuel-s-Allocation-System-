using IAS.Application.Unavailabilities;
using IAS.Domain.Unavailabilities;
using MediatR;

namespace IAS.Application.Unavailabilities.Commands.CreateUnavailability;

public sealed record CreateUnavailabilityCommand(
    Guid PersonId,
    DateOnly StartDate,
    DateOnly EndDate,
    UnavailabilityType Type,
    string? Notes) : IRequest<UnavailabilityDto>;
