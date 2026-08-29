using FluentValidation;

namespace IAS.Application.People.Commands.UpdatePersonSkill;

public sealed class UpdatePersonSkillCommandValidator : AbstractValidator<UpdatePersonSkillCommand>
{
    public UpdatePersonSkillCommandValidator()
    {
        RuleFor(x => x.PersonId).NotEmpty();
        RuleFor(x => x.PersonSkillId).NotEmpty();
        RuleFor(x => x.Level).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
