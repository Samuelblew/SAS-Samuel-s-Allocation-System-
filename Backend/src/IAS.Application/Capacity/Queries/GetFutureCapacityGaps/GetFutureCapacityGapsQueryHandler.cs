using MediatR;

namespace IAS.Application.Capacity.Queries.GetFutureCapacityGaps;

public sealed class GetFutureCapacityGapsQueryHandler(ICapacityReadRepository repository)
    : IRequestHandler<GetFutureCapacityGapsQuery, FutureCapacityGapsDto>
{
    public async Task<FutureCapacityGapsDto> Handle(
        GetFutureCapacityGapsQuery request,
        CancellationToken cancellationToken)
    {
        var needs = await repository.ListAllocationNeedsForActiveProjectsAsync(cancellationToken);
        var allocations = await repository.GetAllAllocationsAsync(cancellationToken);
        var capacityData = await CapacityDataLoader.LoadAsync(
            repository,
            request.From,
            request.To,
            includeSkills: false,
            cancellationToken);

        var result = FutureCapacityGapsCalculator.Calculate(
            request.From,
            request.To,
            needs,
            allocations,
            capacityData);

        return new FutureCapacityGapsDto(
            result.From,
            result.To,
            result.PeakShortfallPercent,
            result.Weeks.Select(w => new WeekCapacityGapDto(
                w.WeekStart,
                w.WeekEnd,
                w.TotalGapDemandPercent,
                w.TotalAvailableSupplyPercent,
                w.NetShortfallPercent,
                w.OpenNeedsInWeek)).ToList(),
            result.OpenNeeds.Select(n => new OpenNeedGapDto(
                n.NeedId,
                n.ProjectId,
                n.ProjectName,
                n.Role,
                n.RequiredPercent,
                n.CoveredPercent,
                n.GapPercent,
                n.Status,
                n.StartDate,
                n.EndDate)).ToList());
    }
}
