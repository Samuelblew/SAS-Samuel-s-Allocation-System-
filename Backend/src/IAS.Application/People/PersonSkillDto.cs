using IAS.Domain.People;

namespace IAS.Application.People;

public sealed record PersonSkillDto(
    Guid Id,
    Guid SkillId,
    string SkillName,
    string? SkillCategory,
    SkillProficiencyLevel Level,
    DateTime? LastUsedAt,
    string? Notes);
