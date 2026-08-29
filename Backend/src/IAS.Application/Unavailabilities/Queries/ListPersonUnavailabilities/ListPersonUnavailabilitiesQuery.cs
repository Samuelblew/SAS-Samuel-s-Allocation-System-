using IAS.Application.Common.Models;
using IAS.Application.Unavailabilities;
using MediatR;

namespace IAS.Application.Unavailabilities.Queries.ListPersonUnavailabilities;

public sealed record ListPersonUnavailabilitiesQuery(
    Guid PersonId,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<UnavailabilityDto>>;
