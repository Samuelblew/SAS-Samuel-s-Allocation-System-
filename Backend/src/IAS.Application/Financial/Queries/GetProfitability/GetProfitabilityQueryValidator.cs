using FluentValidation;

namespace IAS.Application.Financial.Queries.GetProfitability;

public sealed class GetProfitabilityQueryValidator : AbstractValidator<GetProfitabilityQuery>
{
    public GetProfitabilityQueryValidator()
    {
        RuleFor(x => x.MarginAlertThresholdPercent).InclusiveBetween(0, 100);
        RuleFor(x => x)
            .Must(x => !x.From.HasValue || !x.To.HasValue || x.To >= x.From)
            .WithMessage("A data final deve ser maior ou igual à data inicial.");
        RuleFor(x => x.GroupBy).IsInEnum();
    }
}
