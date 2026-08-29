using FluentValidation;

namespace IAS.Application.Unavailabilities.Queries.ListPersonUnavailabilities;

public sealed class ListPersonUnavailabilitiesQueryValidator
    : AbstractValidator<ListPersonUnavailabilitiesQuery>
{
    public ListPersonUnavailabilitiesQueryValidator()
    {
        RuleFor(x => x.PersonId).NotEmpty();
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
