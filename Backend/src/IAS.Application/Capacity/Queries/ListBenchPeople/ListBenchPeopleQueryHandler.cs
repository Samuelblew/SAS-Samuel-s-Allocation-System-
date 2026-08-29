using MediatR;

namespace IAS.Application.Capacity.Queries.ListBenchPeople;

public sealed class ListBenchPeopleQueryHandler(ICapacityReadRepository repository)
    : IRequestHandler<ListBenchPeopleQuery, BenchPeopleListDto>
{
    public async Task<BenchPeopleListDto> Handle(
        ListBenchPeopleQuery request,
        CancellationToken cancellationToken)
    {
        var data = await CapacityDataLoader.LoadAsync(
            repository, request.From, request.To, includeSkills: false, cancellationToken);

        var allocationsByPerson = data.Allocations.GroupBy(a => a.PersonId).ToDictionary(g => g.Key, g => g.ToList());
        var unavailabilitiesByPerson = data.Unavailabilities.GroupBy(u => u.PersonId).ToDictionary(g => g.Key, g => g.ToList());

        var results = new List<BenchPersonDto>();

        foreach (var person in data.People)
        {
            var weeks = PersonAvailabilityCalculator.Calculate(
                request.From, request.To,
                allocationsByPerson.GetValueOrDefault(person.Id) ?? [],
                unavailabilitiesByPerson.GetValueOrDefault(person.Id) ?? []);

            if (weeks.Count == 0)
                continue;

            var minAvailable = weeks.Min(w => w.AvailablePercent);
            if (minAvailable < request.MinAvailablePercent)
                continue;

            results.Add(new BenchPersonDto(
                person.Id,
                person.Name,
                person.Team,
                person.Seniority,
                minAvailable,
                Math.Round(weeks.Average(w => w.AvailablePercent), 2)));
        }

        return new BenchPeopleListDto(
            request.From,
            request.To,
            request.MinAvailablePercent,
            results.OrderByDescending(p => p.AvgAvailablePercent).ToList());
    }
}
