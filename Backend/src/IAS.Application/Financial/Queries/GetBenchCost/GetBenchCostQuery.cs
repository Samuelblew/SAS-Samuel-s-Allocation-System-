using MediatR;

namespace IAS.Application.Financial.Queries.GetBenchCost;

public sealed record GetBenchCostQuery(
    DateOnly From,
    DateOnly To,
    decimal MinAvailablePercent = 50m) : IRequest<BenchCostDto>;

public sealed record BenchCostDto(
    DateOnly From,
    DateOnly To,
    decimal MinAvailablePercent,
    decimal TotalBenchHours,
    decimal TotalBenchCost,
    IReadOnlyList<BenchPersonCostDto> People);

public sealed record BenchPersonCostDto(
    Guid PersonId,
    string PersonName,
    string? Team,
    decimal MinAvailablePercent,
    decimal AvgAvailablePercent,
    decimal BenchHours,
    decimal BenchCost,
    bool HasCostData);
