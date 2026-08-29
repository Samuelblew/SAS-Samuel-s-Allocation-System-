using FluentValidation;

namespace IAS.Application.Capacity.Queries.GetCapacityOverview;

public sealed class GetCapacityOverviewQueryValidator : AbstractValidator<GetCapacityOverviewQuery>
{
    public GetCapacityOverviewQueryValidator()
    {
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From);
        RuleFor(x => x.BenchThreshold).InclusiveBetween(0, 100);
    }
}
