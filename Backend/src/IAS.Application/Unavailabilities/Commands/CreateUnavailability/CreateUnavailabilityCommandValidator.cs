using FluentValidation;

namespace IAS.Application.Unavailabilities.Commands.CreateUnavailability;

public sealed class CreateUnavailabilityCommandValidator : AbstractValidator<CreateUnavailabilityCommand>
{
    public CreateUnavailabilityCommandValidator()
    {
        RuleFor(x => x.PersonId).NotEmpty();
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
