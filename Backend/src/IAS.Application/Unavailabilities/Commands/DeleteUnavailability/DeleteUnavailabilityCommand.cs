using MediatR;

namespace IAS.Application.Unavailabilities.Commands.DeleteUnavailability;

public sealed record DeleteUnavailabilityCommand(Guid PersonId, Guid Id) : IRequest;
