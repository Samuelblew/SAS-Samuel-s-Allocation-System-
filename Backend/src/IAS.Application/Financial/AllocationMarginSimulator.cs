using IAS.Domain.Allocations;
using IAS.Domain.People;
using IAS.Domain.Projects;

namespace IAS.Application.Financial;

public sealed record AllocationMarginSimulationResult(
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

public static class AllocationMarginSimulator
{
    public static AllocationMarginSimulationResult Simulate(
        Project project,
        IReadOnlyList<Allocation> existingAllocations,
        Person proposedPerson,
        string role,
        decimal dedicationPercent,
        DateOnly startDate,
        DateOnly endDate,
        decimal marginAlertThresholdPercent)
    {
        var (periodStart, periodEnd) = FinancialPeriodResolver.Resolve(
            project,
            existingAllocations,
            startDate,
            endDate);

        var existingCosts = existingAllocations
            .Select(a => AllocationCostCalculator.Calculate(a, a.Person, periodStart, periodEnd))
            .ToList();

        var simulatedAllocation = new Allocation
        {
            Id = Guid.Empty,
            TenantId = project.TenantId,
            PersonId = proposedPerson.Id,
            Person = proposedPerson,
            ProjectId = project.Id,
            Role = role,
            DedicationPercent = dedicationPercent,
            StartDate = startDate,
            EndDate = endDate,
            Status = AllocationStatus.Planned,
            CreatedAt = DateTime.UtcNow
        };

        var additionalCost = AllocationCostCalculator.Calculate(
            simulatedAllocation,
            proposedPerson,
            periodStart,
            periodEnd);

        var currentMargin = ProjectMarginCalculator.Calculate(
            project,
            existingCosts,
            marginAlertThresholdPercent);

        var projectedCosts = existingCosts.Append(additionalCost).ToList();
        var projectedMargin = ProjectMarginCalculator.Calculate(
            project,
            projectedCosts,
            marginAlertThresholdPercent);

        var marginDeltaAmount = (projectedMargin.MarginAmount ?? 0) - (currentMargin.MarginAmount ?? 0);
        decimal? marginDeltaPercent = currentMargin.MarginPercent.HasValue && projectedMargin.MarginPercent.HasValue
            ? Math.Round(projectedMargin.MarginPercent.Value - currentMargin.MarginPercent.Value, 2)
            : null;

        return new AllocationMarginSimulationResult(
            project.Id,
            project.Name,
            periodStart,
            periodEnd,
            currentMargin.TotalCost,
            currentMargin.MarginAmount,
            currentMargin.MarginPercent,
            additionalCost.TotalCost,
            projectedMargin.TotalCost,
            projectedMargin.MarginAmount,
            projectedMargin.MarginPercent,
            Math.Round(marginDeltaAmount, 2),
            marginDeltaPercent,
            currentMargin.HasRevenueData,
            currentMargin.IsLowMarginAlert,
            projectedMargin.IsLowMarginAlert,
            marginAlertThresholdPercent);
    }
}
