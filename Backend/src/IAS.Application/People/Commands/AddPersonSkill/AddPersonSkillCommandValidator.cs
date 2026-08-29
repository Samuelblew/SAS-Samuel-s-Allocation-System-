using FluentValidation;

namespace IAS.Application.People.Commands.AddPersonSkill;

public sealed class AddPersonSkillCommandValidator : AbstractValidator<AddPersonSkillCommand>
{
    public AddPersonSkillCommandValidator()
    {
        RuleFor(x => x.PersonId).NotEmpty();
        RuleFor(x => x.SkillId).NotEmpty();
        RuleFor(x => x.Level).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
