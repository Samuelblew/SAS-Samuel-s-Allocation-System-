using FluentValidation;

namespace IAS.Application.Financial.Queries.GetFinancialOverview;

public sealed class GetFinancialOverviewQueryValidator : AbstractValidator<GetFinancialOverviewQuery>
{
    public GetFinancialOverviewQueryValidator()
    {
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From)
            .When(x => x.From.HasValue && x.To.HasValue);
        RuleFor(x => x.MarginAlertThresholdPercent).InclusiveBetween(0, 100);
    }
}
