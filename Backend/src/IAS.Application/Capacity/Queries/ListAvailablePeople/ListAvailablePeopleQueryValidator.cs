using FluentValidation;

namespace IAS.Application.Capacity.Queries.ListAvailablePeople;

public sealed class ListAvailablePeopleQueryValidator : AbstractValidator<ListAvailablePeopleQuery>
{
    public ListAvailablePeopleQueryValidator()
    {
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From);
        RuleFor(x => x.MinAvailablePercent).InclusiveBetween(0, 100);
    }
}
