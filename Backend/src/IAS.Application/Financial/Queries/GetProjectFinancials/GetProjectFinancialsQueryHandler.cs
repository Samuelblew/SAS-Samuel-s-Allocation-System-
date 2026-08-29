using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.Financial.Queries.GetProjectFinancials;

public sealed class GetProjectFinancialsQueryHandler(IFinancialReadRepository repository)
    : IRequestHandler<GetProjectFinancialsQuery, ProjectFinancialsDto>
{
    public async Task<ProjectFinancialsDto> Handle(
        GetProjectFinancialsQuery request,
        CancellationToken cancellationToken)
    {
        var project = await repository.GetProjectWithClientAsync(request.ProjectId, cancellationToken)
            ?? throw new NotFoundException($"Projeto '{request.ProjectId}' não encontrado.");

        var allocations = await repository.GetProjectAllocationsWithPeopleAsync(
            request.ProjectId,
            cancellationToken);

        var (periodStart, periodEnd) = FinancialPeriodResolver.Resolve(
            project,
            allocations,
            request.From,
            request.To);

        var costs = allocations
            .Select(a => AllocationCostCalculator.Calculate(a, a.Person, periodStart, periodEnd))
            .ToList();

        var margin = ProjectMarginCalculator.Calculate(
            project,
            costs,
            request.MarginAlertThresholdPercent);

        return new ProjectFinancialsDto(
            project.Id,
            project.Name,
            project.ClientId,
            project.Client.Name,
            project.Status,
            periodStart,
            periodEnd,
            project.EstimatedRevenue,
            project.Budget,
            margin.TotalCost,
            margin.MarginAmount,
            margin.MarginPercent,
            margin.HasRevenueData,
            margin.HasCostData,
            margin.IsLowMarginAlert,
            request.MarginAlertThresholdPercent,
            costs.Select(Map).ToList());
    }

    private static AllocationCostDto Map(AllocationCostBreakdown cost) =>
        new(
            cost.AllocationId,
            cost.PersonId,
            cost.PersonName,
            cost.Role,
            cost.DedicationPercent,
            cost.AllocationStart,
            cost.AllocationEnd,
            cost.HourlyRate,
            cost.WeeksInPeriod,
            cost.TotalHours,
            cost.TotalCost,
            cost.HasCostData);
}
