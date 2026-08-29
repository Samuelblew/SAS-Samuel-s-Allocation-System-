using FluentValidation;

namespace IAS.Application.Financial.Queries.GetBenchCost;

public sealed class GetBenchCostQueryValidator : AbstractValidator<GetBenchCostQuery>
{
    public GetBenchCostQueryValidator()
    {
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From);
        RuleFor(x => x.MinAvailablePercent).InclusiveBetween(0, 100);
    }
}
