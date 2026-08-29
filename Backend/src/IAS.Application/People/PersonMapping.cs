using IAS.Domain.People;

namespace IAS.Application.People;

internal static class PersonMapping
{
    public static PersonDto ToDto(this Person person) =>
        new(
            person.Id,
            person.Name,
            person.JobTitle,
            person.Seniority,
            person.HourlyCost,
            person.MonthlyCost,
            person.WeeklyCapacityHours,
            person.Location,
            person.Team,
            person.Status,
            person.Skills.Select(s => s.ToDto()).ToList(),
            person.CreatedAt,
            person.UpdatedAt);

    public static PersonListItemDto ToListItemDto(this Person person) =>
        new(
            person.Id,
            person.Name,
            person.JobTitle,
            person.Seniority,
            person.WeeklyCapacityHours,
            person.Status,
            person.Skills.Count,
            person.CreatedAt);

    public static PersonSkillDto ToDto(this PersonSkill personSkill) =>
        new(
            personSkill.Id,
            personSkill.SkillId,
            personSkill.Skill.Name,
            personSkill.Skill.Category,
            personSkill.Level,
            personSkill.LastUsedAt,
            personSkill.Notes);
}
