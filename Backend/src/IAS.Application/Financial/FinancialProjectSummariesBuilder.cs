using IAS.Domain.Allocations;
using IAS.Domain.Projects;

namespace IAS.Application.Financial;

public static class FinancialProjectSummariesBuilder
{
    public static async Task<FinancialSummariesBuildResult> BuildAsync(
        IFinancialReadRepository repository,
        DateOnly? from,
        DateOnly? to,
        decimal marginAlertThresholdPercent,
        CancellationToken cancellationToken)
    {
        var projects = await repository.ListActiveProjectsWithClientAsync(cancellationToken);
        var projectIds = projects.Select(p => p.Id).ToList();
        var allAllocations = await repository.GetAllocationsForProjectsAsync(projectIds, cancellationToken);
        var allocationsByProject = allAllocations.GroupBy(a => a.ProjectId).ToDictionary(g => g.Key, g => g.ToList());

        var globalFrom = from;
        var globalTo = to;

        if (!globalFrom.HasValue || !globalTo.HasValue)
        {
            var allAllocationsList = allAllocations.ToList();
            var defaultFrom = globalFrom
                ?? (allAllocationsList.Count > 0
                    ? allAllocationsList.Min(a => a.StartDate)
                    : DateOnly.FromDateTime(DateTime.UtcNow));
            var defaultTo = globalTo
                ?? (allAllocationsList.Count > 0
                    ? allAllocationsList.Max(a => a.EndDate)
                    : defaultFrom.AddMonths(3));

            if (defaultTo < defaultFrom)
                defaultTo = defaultFrom.AddMonths(3);

            globalFrom = defaultFrom;
            globalTo = defaultTo;
        }

        var summaries = new List<ProjectFinancialDetailDto>();

        foreach (var project in projects)
        {
            var projectAllocations = allocationsByProject.GetValueOrDefault(project.Id) ?? [];
            var (periodStart, periodEnd) = FinancialPeriodResolver.Resolve(
                project,
                projectAllocations,
                globalFrom,
                globalTo);

            var costs = projectAllocations
                .Select(a => AllocationCostCalculator.Calculate(a, a.Person, periodStart, periodEnd))
                .ToList();

            var margin = ProjectMarginCalculator.Calculate(
                project,
                costs,
                marginAlertThresholdPercent);

            summaries.Add(new ProjectFinancialDetailDto(
                project.Id,
                project.Name,
                project.ClientId,
                project.Client.Name,
                project.ProjectType,
                project.Status,
                margin.Revenue,
                margin.TotalCost,
                margin.MarginAmount,
                margin.MarginPercent,
                margin.IsLowMarginAlert));
        }

        return new FinancialSummariesBuildResult(globalFrom!.Value, globalTo!.Value, summaries);
    }
}

public sealed record FinancialSummariesBuildResult(
    DateOnly PeriodStart,
    DateOnly PeriodEnd,
    IReadOnlyList<ProjectFinancialDetailDto> Projects);

public sealed record ProjectFinancialDetailDto(
    Guid ProjectId,
    string ProjectName,
    Guid ClientId,
    string ClientName,
    string? ProjectType,
    ProjectStatus Status,
    decimal? EstimatedRevenue,
    decimal TotalCost,
    decimal? MarginAmount,
    decimal? MarginPercent,
    bool IsLowMarginAlert);
