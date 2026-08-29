using MediatR;

namespace IAS.Application.Capacity.Queries.ListBenchPeople;

public sealed record ListBenchPeopleQuery(
    DateOnly From,
    DateOnly To,
    decimal MinAvailablePercent = CapacityOverviewCalculator.DefaultBenchThreshold) : IRequest<BenchPeopleListDto>;

public sealed record BenchPeopleListDto(
    DateOnly From,
    DateOnly To,
    decimal MinAvailablePercent,
    IReadOnlyList<BenchPersonDto> People);

public sealed record BenchPersonDto(
    Guid PersonId,
    string PersonName,
    string? Team,
    string? Seniority,
    decimal MinAvailablePercentInPeriod,
    decimal AvgAvailablePercent);
