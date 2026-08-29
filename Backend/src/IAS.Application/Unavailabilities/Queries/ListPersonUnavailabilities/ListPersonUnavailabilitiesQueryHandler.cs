using IAS.Application.Common.Exceptions;
using IAS.Application.Common.Models;
using IAS.Application.Unavailabilities;
using MediatR;

namespace IAS.Application.Unavailabilities.Queries.ListPersonUnavailabilities;

public sealed class ListPersonUnavailabilitiesQueryHandler(IUnavailabilityRepository repository)
    : IRequestHandler<ListPersonUnavailabilitiesQuery, PagedResult<UnavailabilityDto>>
{
    public async Task<PagedResult<UnavailabilityDto>> Handle(
        ListPersonUnavailabilitiesQuery request,
        CancellationToken cancellationToken)
    {
        if (!await repository.PersonExistsAsync(request.PersonId, cancellationToken))
            throw new NotFoundException($"Pessoa '{request.PersonId}' não encontrada.");

        var (items, total) = await repository.ListByPersonAsync(
            request.PersonId,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new PagedResult<UnavailabilityDto>(
            items.Select(i => i.ToDto()).ToList(),
            request.Page,
            request.PageSize,
            total);
    }
}
