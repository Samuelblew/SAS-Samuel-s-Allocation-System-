namespace IAS.Application.Skills;

public sealed record SkillDto(
    Guid Id,
    string Name,
    string? Category,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
