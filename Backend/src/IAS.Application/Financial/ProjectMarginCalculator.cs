using IAS.Domain.Projects;

namespace IAS.Application.Financial;

public sealed record ProjectMarginResult(
    decimal? Revenue,
    decimal? Budget,
    decimal TotalCost,
    decimal? MarginAmount,
    decimal? MarginPercent,
    bool HasRevenueData,
    bool HasCostData,
    bool IsLowMarginAlert);

public static class ProjectMarginCalculator
{
    public static ProjectMarginResult Calculate(
        Project project,
        IReadOnlyList<AllocationCostBreakdown> allocationCosts,
        decimal marginAlertThresholdPercent)
    {
        var revenue = project.EstimatedRevenue ?? project.Budget;
        var totalCost = allocationCosts.Sum(a => a.TotalCost);
        var hasCostData = allocationCosts.Any(a => a.HasCostData);
        var hasRevenue = revenue.HasValue && revenue.Value > 0;

        decimal? marginAmount = hasRevenue ? revenue!.Value - totalCost : null;
        decimal? marginPercent = hasRevenue && marginAmount.HasValue
            ? Math.Round(marginAmount.Value / revenue!.Value * 100m, 2)
            : null;

        var isAlert = marginPercent.HasValue && marginPercent.Value < marginAlertThresholdPercent;

        return new ProjectMarginResult(
            project.EstimatedRevenue,
            project.Budget,
            Math.Round(totalCost, 2),
            marginAmount.HasValue ? Math.Round(marginAmount.Value, 2) : null,
            marginPercent,
            hasRevenue,
            hasCostData,
            isAlert);
    }
}
