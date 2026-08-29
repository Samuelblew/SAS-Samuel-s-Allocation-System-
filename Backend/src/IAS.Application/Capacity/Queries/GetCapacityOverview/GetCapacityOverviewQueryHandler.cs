using MediatR;

namespace IAS.Application.Capacity.Queries.GetCapacityOverview;

public sealed class GetCapacityOverviewQueryHandler(ICapacityReadRepository repository)
    : IRequestHandler<GetCapacityOverviewQuery, CapacityOverviewDto>
{
    public async Task<CapacityOverviewDto> Handle(
        GetCapacityOverviewQuery request,
        CancellationToken cancellationToken)
    {
        var data = await CapacityDataLoader.LoadAsync(
            repository, request.From, request.To, includeSkills: false, cancellationToken);

        var weeks = CapacityOverviewCalculator.CalculateWeeklyOverview(
            request.From, request.To, data, request.BenchThreshold);

        var teams = CapacityOverviewCalculator.CalculateTeamOccupation(
            request.From, request.To, data);

        return new CapacityOverviewDto(
            request.From,
            request.To,
            weeks.Select(w => new WeekOverviewDto(
                w.WeekStart, w.WeekEnd, w.ActivePeopleCount,
                w.AvgAllocatedPercent, w.AvgAvailablePercent,
                w.BenchPeopleCount, w.OverallocatedPeopleCount,
                w.TotalCapacityHours, w.TotalAllocatedHours, w.TotalAvailableHours)).ToList(),
            teams.Select(t => new TeamOccupationDto(
                t.Team, t.PeopleCount, t.AvgAllocatedPercent, t.AvgAvailablePercent)).ToList());
    }
}
