using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.Capacity.Queries.ListAvailablePeople;

public sealed class ListAvailablePeopleQueryHandler(ICapacityReadRepository repository)
    : IRequestHandler<ListAvailablePeopleQuery, AvailablePeopleListDto>
{
    public async Task<AvailablePeopleListDto> Handle(
        ListAvailablePeopleQuery request,
        CancellationToken cancellationToken)
    {
        var people = await repository.ListActivePeopleAsync(cancellationToken);
        var results = new List<AvailablePersonDto>();

        foreach (var person in people)
        {
            var allocations = await repository.GetAllocationsForPersonAsync(
                person.Id, request.From, request.To, cancellationToken);
            var unavailabilities = await repository.GetUnavailabilitiesForPersonAsync(
                person.Id, request.From, request.To, cancellationToken);

            var weeks = PersonAvailabilityCalculator.Calculate(
                request.From, request.To, allocations, unavailabilities);

            if (weeks.Count == 0)
                continue;

            var minAvailable = weeks.Min(w => w.AvailablePercent);
            if (minAvailable < request.MinAvailablePercent)
                continue;

            var avgAvailable = weeks.Average(w => w.AvailablePercent);

            results.Add(new AvailablePersonDto(
                person.Id,
                person.Name,
                minAvailable,
                Math.Round(avgAvailable, 2)));
        }

        return new AvailablePeopleListDto(
            request.From,
            request.To,
            results.OrderByDescending(p => p.MinAvailablePercentInPeriod).ToList());
    }
}
