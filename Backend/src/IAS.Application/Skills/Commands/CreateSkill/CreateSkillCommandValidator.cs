using FluentValidation;

namespace IAS.Application.Skills.Commands.CreateSkill;

public sealed class CreateSkillCommandValidator : AbstractValidator<CreateSkillCommand>
{
    public CreateSkillCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(120);

        RuleFor(x => x.Category)
            .MaximumLength(80)
            .When(x => x.Category is not null);
    }
}
