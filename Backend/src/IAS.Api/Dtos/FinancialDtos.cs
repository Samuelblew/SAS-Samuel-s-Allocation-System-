using IAS.Domain.Projects;

namespace IAS.Api.Dtos;

public sealed record ProjectFinancialsResponse(
    Guid ProjectId,
    string ProjectName,
    Guid ClientId,
    string ClientName,
    ProjectStatus Status,
    DateOnly PeriodStart,
    DateOnly PeriodEnd,
    decimal? EstimatedRevenue,
    decimal? Budget,
    decimal TotalCost,
    decimal? MarginAmount,
    decimal? MarginPercent,
    bool HasRevenueData,
    bool HasCostData,
    bool IsLowMarginAlert,
    decimal MarginAlertThresholdPercent,
    IReadOnlyList<AllocationCostResponse> Allocations);

public sealed record AllocationCostResponse(
    Guid AllocationId,
    Guid PersonId,
    string PersonName,
    string Role,
    decimal DedicationPercent,
    DateOnly AllocationStart,
    DateOnly AllocationEnd,
    decimal? HourlyRate,
    int WeeksInPeriod,
    decimal TotalHours,
    decimal TotalCost,
    bool HasCostData);

public sealed record FinancialOverviewResponse(
    DateOnly PeriodStart,
    DateOnly PeriodEnd,
    decimal MarginAlertThresholdPercent,
    decimal TotalCost,
    decimal? TotalRevenue,
    decimal? TotalMargin,
    decimal? AvgMarginPercent,
    IReadOnlyList<ProjectFinancialSummaryResponse> Projects,
    IReadOnlyList<LowMarginAlertResponse> LowMarginAlerts);

public sealed record ProjectFinancialSummaryResponse(
    Guid ProjectId,
    string ProjectName,
    string ClientName,
    ProjectStatus Status,
    decimal? EstimatedRevenue,
    decimal TotalCost,
    decimal? MarginAmount,
    decimal? MarginPercent,
    bool IsLowMarginAlert);

public sealed record LowMarginAlertResponse(
    Guid ProjectId,
    string ProjectName,
    string ClientName,
    decimal? MarginPercent,
    decimal TotalCost,
    decimal? EstimatedRevenue);

public sealed record BenchCostResponse(
    DateOnly From,
    DateOnly To,
    decimal MinAvailablePercent,
    decimal TotalBenchHours,
    decimal TotalBenchCost,
    IReadOnlyList<BenchPersonCostResponse> People);

public sealed record BenchPersonCostResponse(
    Guid PersonId,
    string PersonName,
    string? Team,
    decimal MinAvailablePercent,
    decimal AvgAvailablePercent,
    decimal BenchHours,
    decimal BenchCost,
    bool HasCostData);

public sealed record ProfitabilityResponse(
    DateOnly PeriodStart,
    DateOnly PeriodEnd,
    string GroupBy,
    decimal MarginAlertThresholdPercent,
    IReadOnlyList<ProfitabilityGroupResponse> Groups);

public sealed record ProfitabilityGroupResponse(
    string GroupKey,
    Guid? ClientId,
    int ProjectCount,
    decimal TotalCost,
    decimal? TotalRevenue,
    decimal? TotalMargin,
    decimal? MarginPercent,
    bool IsLowMarginAlert);
