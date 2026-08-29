using FluentValidation;

namespace IAS.Application.Skills.Queries.ListSkills;

public sealed class ListSkillsQueryValidator : AbstractValidator<ListSkillsQuery>
{
    public ListSkillsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
