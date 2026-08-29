using MediatR;

namespace IAS.Application.Financial.Commands.SimulateAllocationMargin;

public sealed record SimulateAllocationMarginCommand(
    Guid ProjectId,
    Guid PersonId,
    string Role,
    decimal DedicationPercent,
    DateOnly StartDate,
    DateOnly EndDate,
    decimal MarginAlertThresholdPercent = 15m) : IRequest<AllocationMarginSimulationDto>;

public sealed record AllocationMarginSimulationDto(
    Guid ProjectId,
    string ProjectName,
    DateOnly PeriodStart,
    DateOnly PeriodEnd,
    decimal? CurrentTotalCost,
    decimal? CurrentMarginAmount,
    decimal? CurrentMarginPercent,
    decimal SimulatedAdditionalCost,
    decimal? ProjectedTotalCost,
    decimal? ProjectedMarginAmount,
    decimal? ProjectedMarginPercent,
    decimal MarginDeltaAmount,
    decimal? MarginDeltaPercent,
    bool HasRevenueData,
    bool CurrentIsLowMarginAlert,
    bool ProjectedIsLowMarginAlert,
    decimal MarginAlertThresholdPercent);
