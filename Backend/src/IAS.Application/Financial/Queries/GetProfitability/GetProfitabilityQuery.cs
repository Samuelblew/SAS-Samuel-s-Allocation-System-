using MediatR;

namespace IAS.Application.Financial.Queries.GetProfitability;

public enum ProfitabilityGroupBy
{
    Client,
    ProjectType
}

public sealed record GetProfitabilityQuery(
    DateOnly? From = null,
    DateOnly? To = null,
    ProfitabilityGroupBy GroupBy = ProfitabilityGroupBy.Client,
    decimal MarginAlertThresholdPercent = 15m) : IRequest<ProfitabilityDto>;

public sealed record ProfitabilityDto(
    DateOnly PeriodStart,
    DateOnly PeriodEnd,
    ProfitabilityGroupBy GroupBy,
    decimal MarginAlertThresholdPercent,
    IReadOnlyList<ProfitabilityGroupDto> Groups);

public sealed record ProfitabilityGroupDto(
    string GroupKey,
    Guid? ClientId,
    int ProjectCount,
    decimal TotalCost,
    decimal? TotalRevenue,
    decimal? TotalMargin,
    decimal? MarginPercent,
    bool IsLowMarginAlert);
