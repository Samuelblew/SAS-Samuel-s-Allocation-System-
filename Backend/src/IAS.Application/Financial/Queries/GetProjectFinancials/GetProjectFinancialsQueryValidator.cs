using FluentValidation;

namespace IAS.Application.Financial.Queries.GetProjectFinancials;

public sealed class GetProjectFinancialsQueryValidator : AbstractValidator<GetProjectFinancialsQuery>
{
    public GetProjectFinancialsQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From)
            .When(x => x.From.HasValue && x.To.HasValue);
        RuleFor(x => x.MarginAlertThresholdPercent).InclusiveBetween(0, 100);
    }
}
