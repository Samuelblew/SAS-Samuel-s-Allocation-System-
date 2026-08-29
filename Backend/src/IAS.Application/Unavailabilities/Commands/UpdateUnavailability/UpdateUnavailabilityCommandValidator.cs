using FluentValidation;

namespace IAS.Application.Unavailabilities.Commands.UpdateUnavailability;

public sealed class UpdateUnavailabilityCommandValidator : AbstractValidator<UpdateUnavailabilityCommand>
{
    public UpdateUnavailabilityCommandValidator()
    {
        RuleFor(x => x.PersonId).NotEmpty();
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
