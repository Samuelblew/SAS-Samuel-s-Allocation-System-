using FluentValidation;

namespace IAS.Application.Capacity.Queries.GetFutureCapacityGaps;

public sealed class GetFutureCapacityGapsQueryValidator : AbstractValidator<GetFutureCapacityGapsQuery>
{
    public GetFutureCapacityGapsQueryValidator()
    {
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From);
    }
}
