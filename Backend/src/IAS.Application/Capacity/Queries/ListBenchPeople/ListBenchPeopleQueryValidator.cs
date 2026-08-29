using FluentValidation;

namespace IAS.Application.Capacity.Queries.ListBenchPeople;

public sealed class ListBenchPeopleQueryValidator : AbstractValidator<ListBenchPeopleQuery>
{
    public ListBenchPeopleQueryValidator()
    {
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From);
        RuleFor(x => x.MinAvailablePercent).InclusiveBetween(0, 100);
    }
}
