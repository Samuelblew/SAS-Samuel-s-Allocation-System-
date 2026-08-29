using IAS.Domain.Projects;
using MediatR;

namespace IAS.Application.Financial.Queries.GetProjectFinancials;

public sealed record GetProjectFinancialsQuery(
    Guid ProjectId,
    DateOnly? From = null,
    DateOnly? To = null,
    decimal MarginAlertThresholdPercent = 15m) : IRequest<ProjectFinancialsDto>;

public sealed record ProjectFinancialsDto(
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
    IReadOnlyList<AllocationCostDto> Allocations);

public sealed record AllocationCostDto(
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
