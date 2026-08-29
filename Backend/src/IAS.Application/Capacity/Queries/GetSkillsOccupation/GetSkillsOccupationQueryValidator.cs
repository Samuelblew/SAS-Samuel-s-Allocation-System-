using FluentValidation;

namespace IAS.Application.Capacity.Queries.GetSkillsOccupation;

public sealed class GetSkillsOccupationQueryValidator : AbstractValidator<GetSkillsOccupationQuery>
{
    public GetSkillsOccupationQueryValidator()
    {
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From);
    }
}
