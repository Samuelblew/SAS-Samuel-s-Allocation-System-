using IAS.Domain.AllocationNeeds;
using MediatR;

namespace IAS.Application.Capacity.Queries.GetFutureCapacityGaps;

public sealed record GetFutureCapacityGapsQuery(DateOnly From, DateOnly To) : IRequest<FutureCapacityGapsDto>;

public sealed record FutureCapacityGapsDto(
    DateOnly From,
    DateOnly To,
    decimal PeakShortfallPercent,
    IReadOnlyList<WeekCapacityGapDto> Weeks,
    IReadOnlyList<OpenNeedGapDto> OpenNeeds);

public sealed record WeekCapacityGapDto(
    DateOnly WeekStart,
    DateOnly WeekEnd,
    decimal TotalGapDemandPercent,
    decimal TotalAvailableSupplyPercent,
    decimal NetShortfallPercent,
    int OpenNeedsInWeek);

public sealed record OpenNeedGapDto(
    Guid NeedId,
    Guid ProjectId,
    string ProjectName,
    string Role,
    decimal RequiredPercent,
    decimal CoveredPercent,
    decimal GapPercent,
    AllocationNeedStatus Status,
    DateOnly? StartDate,
    DateOnly? EndDate);
