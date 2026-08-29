using MediatR;

namespace IAS.Application.Financial.Queries.GetProfitability;

public sealed class GetProfitabilityQueryHandler(IFinancialReadRepository repository)
    : IRequestHandler<GetProfitabilityQuery, ProfitabilityDto>
{
    public async Task<ProfitabilityDto> Handle(
        GetProfitabilityQuery request,
        CancellationToken cancellationToken)
    {
        var build = await FinancialProjectSummariesBuilder.BuildAsync(
            repository,
            request.From,
            request.To,
            request.MarginAlertThresholdPercent,
            cancellationToken);

        var groups = request.GroupBy switch
        {
            ProfitabilityGroupBy.Client => GroupByClient(build.Projects, request.MarginAlertThresholdPercent),
            ProfitabilityGroupBy.ProjectType => GroupByProjectType(build.Projects, request.MarginAlertThresholdPercent),
            _ => GroupByClient(build.Projects, request.MarginAlertThresholdPercent)
        };

        return new ProfitabilityDto(
            build.PeriodStart,
            build.PeriodEnd,
            request.GroupBy,
            request.MarginAlertThresholdPercent,
            groups);
    }

    private static IReadOnlyList<ProfitabilityGroupDto> GroupByClient(
        IReadOnlyList<ProjectFinancialDetailDto> projects,
        decimal marginAlertThresholdPercent)
    {
        return projects
            .GroupBy(p => new { p.ClientId, p.ClientName })
            .Select(g => Aggregate(
                g.Key.ClientName,
                g.Key.ClientId,
                g.ToList(),
                marginAlertThresholdPercent))
            .OrderByDescending(g => g.IsLowMarginAlert)
            .ThenBy(g => g.MarginPercent)
            .ThenBy(g => g.GroupKey)
            .ToList();
    }

    private static IReadOnlyList<ProfitabilityGroupDto> GroupByProjectType(
        IReadOnlyList<ProjectFinancialDetailDto> projects,
        decimal marginAlertThresholdPercent)
    {
        return projects
            .GroupBy(p => string.IsNullOrWhiteSpace(p.ProjectType) ? "Sem tipo" : p.ProjectType!.Trim())
            .Select(g => Aggregate(
                g.Key,
                null,
                g.ToList(),
                marginAlertThresholdPercent))
            .OrderByDescending(g => g.IsLowMarginAlert)
            .ThenBy(g => g.MarginPercent)
            .ThenBy(g => g.GroupKey)
            .ToList();
    }

    private static ProfitabilityGroupDto Aggregate(
        string groupKey,
        Guid? clientId,
        IReadOnlyList<ProjectFinancialDetailDto> projects,
        decimal marginAlertThresholdPercent)
    {
        var totalCost = Math.Round(projects.Sum(p => p.TotalCost), 2);
        var withRevenue = projects.Where(p => p.EstimatedRevenue.HasValue).ToList();
        var totalRevenue = withRevenue.Count > 0
            ? withRevenue.Sum(p => p.EstimatedRevenue!.Value)
            : (decimal?)null;
        var totalMargin = totalRevenue.HasValue
            ? Math.Round(totalRevenue.Value - totalCost, 2)
            : (decimal?)null;
        var marginPercent = totalRevenue is > 0 && totalMargin.HasValue
            ? Math.Round(totalMargin.Value / totalRevenue.Value * 100m, 2)
            : (decimal?)null;
        var isLowMargin = marginPercent.HasValue && marginPercent.Value < marginAlertThresholdPercent;

        return new ProfitabilityGroupDto(
            groupKey,
            clientId,
            projects.Count,
            totalCost,
            totalRevenue,
            totalMargin,
            marginPercent,
            isLowMargin);
    }
}
