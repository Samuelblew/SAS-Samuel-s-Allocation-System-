using MediatR;

namespace IAS.Application.Financial.Queries.GetFinancialOverview;

public sealed class GetFinancialOverviewQueryHandler(IFinancialReadRepository repository)
    : IRequestHandler<GetFinancialOverviewQuery, FinancialOverviewDto>
{
    public async Task<FinancialOverviewDto> Handle(
        GetFinancialOverviewQuery request,
        CancellationToken cancellationToken)
    {
        var build = await FinancialProjectSummariesBuilder.BuildAsync(
            repository,
            request.From,
            request.To,
            request.MarginAlertThresholdPercent,
            cancellationToken);

        var summaries = build.Projects
            .Select(p => new ProjectFinancialSummaryDto(
                p.ProjectId,
                p.ProjectName,
                p.ClientName,
                p.Status,
                p.EstimatedRevenue,
                p.TotalCost,
                p.MarginAmount,
                p.MarginPercent,
                p.IsLowMarginAlert))
            .OrderByDescending(s => s.IsLowMarginAlert)
            .ThenBy(s => s.MarginPercent)
            .ThenBy(s => s.ProjectName)
            .ToList();

        var alerts = summaries
            .Where(s => s.IsLowMarginAlert)
            .Select(s => new LowMarginAlertDto(
                s.ProjectId,
                s.ProjectName,
                s.ClientName,
                s.MarginPercent,
                s.TotalCost,
                s.EstimatedRevenue))
            .OrderBy(a => a.MarginPercent)
            .ThenBy(a => a.ProjectName)
            .ToList();

        var projectsWithRevenue = summaries.Where(s => s.EstimatedRevenue.HasValue).ToList();
        var totalRevenue = projectsWithRevenue.Sum(s => s.EstimatedRevenue!.Value);
        var totalCost = summaries.Sum(s => s.TotalCost);
        var totalMargin = projectsWithRevenue.Count > 0 ? totalRevenue - totalCost : (decimal?)null;
        var avgMarginPercent = projectsWithRevenue.Count > 0 && totalRevenue > 0 && totalMargin.HasValue
            ? Math.Round(totalMargin.Value / totalRevenue * 100m, 2)
            : (decimal?)null;

        return new FinancialOverviewDto(
            build.PeriodStart,
            build.PeriodEnd,
            request.MarginAlertThresholdPercent,
            Math.Round(totalCost, 2),
            projectsWithRevenue.Count > 0 ? totalRevenue : null,
            totalMargin.HasValue ? Math.Round(totalMargin.Value, 2) : null,
            avgMarginPercent,
            summaries,
            alerts);
    }
}
