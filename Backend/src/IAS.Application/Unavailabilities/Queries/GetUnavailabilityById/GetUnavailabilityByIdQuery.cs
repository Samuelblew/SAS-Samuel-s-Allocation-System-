using IAS.Application.Unavailabilities;
using MediatR;

namespace IAS.Application.Unavailabilities.Queries.GetUnavailabilityById;

public sealed record GetUnavailabilityByIdQuery(Guid PersonId, Guid Id) : IRequest<UnavailabilityDto>;
