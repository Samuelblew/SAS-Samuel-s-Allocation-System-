using MediatR;

namespace IAS.Application.Capacity.Queries.GetSkillsOccupation;

public sealed record GetSkillsOccupationQuery(DateOnly From, DateOnly To) : IRequest<SkillsOccupationDto>;

public sealed record SkillsOccupationDto(
    DateOnly From,
    DateOnly To,
    IReadOnlyList<SkillOccupationItemDto> Skills);

public sealed record SkillOccupationItemDto(
    Guid SkillId,
    string SkillName,
    string? Category,
    int PeopleCount,
    decimal AvgAllocatedPercent,
    decimal AvgAvailablePercent,
    decimal AvgAllocatedHours,
    decimal AvgAvailableHours);
