using FluentValidation;

namespace IAS.Application.Skills.Commands.UpdateSkill;

public sealed class UpdateSkillCommandValidator : AbstractValidator<UpdateSkillCommand>
{
    public UpdateSkillCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(x => x.Category)
            .MaximumLength(80)
            .When(x => x.Category is not null);
    }
}
