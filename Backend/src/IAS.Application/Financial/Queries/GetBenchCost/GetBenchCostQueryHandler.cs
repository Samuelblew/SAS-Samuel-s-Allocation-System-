using IAS.Application.Capacity;
using MediatR;

namespace IAS.Application.Financial.Queries.GetBenchCost;

public sealed class GetBenchCostQueryHandler(ICapacityReadRepository capacityRepository)
    : IRequestHandler<GetBenchCostQuery, BenchCostDto>
{
    public async Task<BenchCostDto> Handle(GetBenchCostQuery request, CancellationToken cancellationToken)
    {
        var data = await CapacityDataLoader.LoadAsync(
            capacityRepository,
            request.From,
            request.To,
            includeSkills: false,
            cancellationToken);

        var result = BenchCostCalculator.Calculate(
            request.From,
            request.To,
            request.MinAvailablePercent,
            data);

        return new BenchCostDto(
            result.From,
            result.To,
            result.MinAvailablePercent,
            result.TotalBenchHours,
            result.TotalBenchCost,
            result.People.Select(p => new BenchPersonCostDto(
                p.PersonId,
                p.PersonName,
                p.Team,
                p.MinAvailablePercent,
                p.AvgAvailablePercent,
                p.BenchHours,
                p.BenchCost,
                p.HasCostData)).ToList());
    }
}
