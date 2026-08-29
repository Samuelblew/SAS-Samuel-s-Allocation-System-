using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.Capacity.Queries.GetPersonAvailability;

public sealed class GetPersonAvailabilityQueryHandler(ICapacityReadRepository repository)
    : IRequestHandler<GetPersonAvailabilityQuery, PersonAvailabilityDto>
{
    public async Task<PersonAvailabilityDto> Handle(
        GetPersonAvailabilityQuery request,
        CancellationToken cancellationToken)
    {
        var person = await repository.GetPersonAsync(request.PersonId, cancellationToken)
            ?? throw new NotFoundException($"Pessoa '{request.PersonId}' não encontrada.");

        var allocations = await repository.GetAllocationsForPersonAsync(
            request.PersonId, request.From, request.To, cancellationToken);
        var unavailabilities = await repository.GetUnavailabilitiesForPersonAsync(
            request.PersonId, request.From, request.To, cancellationToken);

        var weeks = PersonAvailabilityCalculator.Calculate(
            request.From, request.To, allocations, unavailabilities);

        return new PersonAvailabilityDto(
            person.Id,
            person.Name,
            person.WeeklyCapacityHours,
            request.From,
            request.To,
            weeks.Select(w => MapWeek(w, person.WeeklyCapacityHours)).ToList());
    }

    private static WeekAvailabilityDto MapWeek(WeekAvailability week, decimal weeklyCapacityHours)
    {
        var hours = EffectiveCapacityCalculator.FromWeek(week, weeklyCapacityHours);
        return new WeekAvailabilityDto(
            week.WeekStart,
            week.WeekEnd,
            week.AllocatedPercent,
            week.AvailablePercent,
            hours.WeeklyCapacityHours,
            hours.AllocatedHours,
            hours.AvailableHours,
            week.IsUnavailable);
    }
}
