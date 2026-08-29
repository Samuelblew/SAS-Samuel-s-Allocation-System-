using FluentValidation;

namespace IAS.Application.People.Commands.UpdatePerson;

public sealed class UpdatePersonCommandValidator : AbstractValidator<UpdatePersonCommand>
{
    public UpdatePersonCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.JobTitle).MaximumLength(120);
        RuleFor(x => x.Seniority).MaximumLength(80);
        RuleFor(x => x.Location).MaximumLength(120);
        RuleFor(x => x.Team).MaximumLength(120);
        RuleFor(x => x.WeeklyCapacityHours).GreaterThan(0).LessThanOrEqualTo(168);
        RuleFor(x => x.HourlyCost).GreaterThanOrEqualTo(0).When(x => x.HourlyCost.HasValue);
        RuleFor(x => x.MonthlyCost).GreaterThanOrEqualTo(0).When(x => x.MonthlyCost.HasValue);
        RuleFor(x => x.Status).IsInEnum();
    }
}
