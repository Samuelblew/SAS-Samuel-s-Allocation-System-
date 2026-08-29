using IAS.Domain.People;

namespace IAS.Api.Dtos;

public sealed record CreatePersonRequest(
    string Name,
    string? JobTitle,
    string? Seniority,
    decimal? HourlyCost,
    decimal? MonthlyCost,
    decimal WeeklyCapacityHours,
    string? Location,
    string? Team,
    PersonStatus Status);

public sealed record UpdatePersonRequest(
    string Name,
    string? JobTitle,
    string? Seniority,
    decimal? HourlyCost,
    decimal? MonthlyCost,
    decimal WeeklyCapacityHours,
    string? Location,
    string? Team,
    PersonStatus Status);

public sealed record PersonSkillResponse(
    Guid Id,
    Guid SkillId,
    string SkillName,
    string? SkillCategory,
    SkillProficiencyLevel Level,
    DateTime? LastUsedAt,
    string? Notes);

public sealed record PersonResponse(
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
    IReadOnlyList<PersonSkillResponse> Skills,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public sealed record PersonListItemResponse(
    Guid Id,
    string Name,
    string? JobTitle,
    string? Seniority,
    decimal WeeklyCapacityHours,
    PersonStatus Status,
    int SkillCount,
    DateTime CreatedAt);

public sealed record PagedPeopleResponse(
    IReadOnlyList<PersonListItemResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);

public sealed record AddPersonSkillRequest(
    Guid SkillId,
    SkillProficiencyLevel Level,
    DateTime? LastUsedAt,
    string? Notes);

public sealed record UpdatePersonSkillRequest(
    SkillProficiencyLevel Level,
    DateTime? LastUsedAt,
    string? Notes);
