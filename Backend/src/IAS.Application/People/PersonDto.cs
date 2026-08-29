using IAS.Domain.People;

namespace IAS.Application.People;

public sealed record PersonDto(
    Guid Id,
    string Name,
    string? JobTitle,
    string? Seniority,
    decimal? HourlyCost,
    decimal? MonthlyCost,
    decimal WeeklyCapacityHours,
    string? Location,
    string? Team,
    PersonStatus Status,
    IReadOnlyList<PersonSkillDto> Skills,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record PersonListItemDto(
    Guid Id,
    string Name,
    string? JobTitle,
    string? Seniority,
    decimal WeeklyCapacityHours,
    PersonStatus Status,
    int SkillCount,
    DateTime CreatedAt);
