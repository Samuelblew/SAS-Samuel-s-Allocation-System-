using MediatR;

namespace IAS.Application.Capacity.Queries.GetCapacityOverview;

public sealed record GetCapacityOverviewQuery(
    DateOnly From,
    DateOnly To,
    decimal BenchThreshold = CapacityOverviewCalculator.DefaultBenchThreshold) : IRequest<CapacityOverviewDto>;

public sealed record CapacityOverviewDto(
    DateOnly From,
    DateOnly To,
    IReadOnlyList<WeekOverviewDto> Weeks,
    IReadOnlyList<TeamOccupationDto> Teams);

public sealed record WeekOverviewDto(
    DateOnly WeekStart,
    DateOnly WeekEnd,
    int ActivePeopleCount,
    decimal AvgAllocatedPercent,
    decimal AvgAvailablePercent,
    int BenchPeopleCount,
    int OverallocatedPeopleCount,
    decimal TotalCapacityHours,
    decimal TotalAllocatedHours,
    decimal TotalAvailableHours);

public sealed record TeamOccupationDto(
    string? Team,
    int PeopleCount,
    decimal AvgAllocatedPercent,
    decimal AvgAvailablePercent);
