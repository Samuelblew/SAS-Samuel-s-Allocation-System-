using IAS.Domain.Projects;
using MediatR;

namespace IAS.Application.Financial.Queries.GetFinancialOverview;

public sealed record GetFinancialOverviewQuery(
    DateOnly? From = null,
    DateOnly? To = null,
    decimal MarginAlertThresholdPercent = 15m) : IRequest<FinancialOverviewDto>;

public sealed record FinancialOverviewDto(
    DateOnly PeriodStart,
    DateOnly PeriodEnd,
    decimal MarginAlertThresholdPercent,
    decimal TotalCost,
    decimal? TotalRevenue,
    decimal? TotalMargin,
    decimal? AvgMarginPercent,
    IReadOnlyList<ProjectFinancialSummaryDto> Projects,
    IReadOnlyList<LowMarginAlertDto> LowMarginAlerts);

public sealed record ProjectFinancialSummaryDto(
    Guid ProjectId,
    string ProjectName,
    string ClientName,
    ProjectStatus Status,
    decimal? EstimatedRevenue,
    decimal TotalCost,
    decimal? MarginAmount,
    decimal? MarginPercent,
    bool IsLowMarginAlert);

public sealed record LowMarginAlertDto(
    Guid ProjectId,
    string ProjectName,
    string ClientName,
    decimal? MarginPercent,
    decimal TotalCost,
    decimal? EstimatedRevenue);
