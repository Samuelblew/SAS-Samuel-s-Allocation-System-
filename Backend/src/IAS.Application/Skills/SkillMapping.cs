using IAS.Domain.Skills;

namespace IAS.Application.Skills;

internal static class SkillMapping
{
    public static SkillDto ToDto(this Skill skill) =>
        new(skill.Id, skill.Name, skill.Category, skill.CreatedAt, skill.UpdatedAt);
}
