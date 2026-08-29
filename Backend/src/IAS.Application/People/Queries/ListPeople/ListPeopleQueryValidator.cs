using FluentValidation;

namespace IAS.Application.People.Queries.ListPeople;

public sealed class ListPeopleQueryValidator : AbstractValidator<ListPeopleQuery>
{
    public ListPeopleQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
