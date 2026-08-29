using FluentValidation;

namespace IAS.Application.Capacity.Queries.GetPersonAvailability;

public sealed class GetPersonAvailabilityQueryValidator : AbstractValidator<GetPersonAvailabilityQuery>
{
    public GetPersonAvailabilityQueryValidator()
    {
        RuleFor(x => x.PersonId).NotEmpty();
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From);
    }
}
