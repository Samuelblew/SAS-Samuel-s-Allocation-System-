using IAS.Api.Dtos;
using IAS.Application.Financial.Queries.GetBenchCost;
using IAS.Application.Financial.Queries.GetFinancialOverview;
using IAS.Application.Financial.Queries.GetProfitability;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IAS.Api.Controllers;

[ApiController]
[Route("api/v1/financials")]
public sealed class FinancialsController(IMediator mediator) : ControllerBase
{
    [HttpGet("bench")]
    [ProducesResponseType(typeof(BenchCostResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BenchCostResponse>> GetBenchCost(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        [FromQuery] decimal minAvailablePercent = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetBenchCostQuery(from, to, minAvailablePercent),
            cancellationToken);

        return Ok(new BenchCostResponse(
            result.From,
            result.To,
            result.MinAvailablePercent,
            result.TotalBenchHours,
            result.TotalBenchCost,
            result.People.Select(p => new BenchPersonCostResponse(
                p.PersonId,
                p.PersonName,
                p.Team,
                p.MinAvailablePercent,
                p.AvgAvailablePercent,
                p.BenchHours,
                p.BenchCost,
                p.HasCostData)).ToList()));
    }

    [HttpGet("profitability")]
    [ProducesResponseType(typeof(ProfitabilityResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ProfitabilityResponse>> GetProfitability(
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        [FromQuery] ProfitabilityGroupBy groupBy = ProfitabilityGroupBy.Client,
        [FromQuery] decimal marginAlertThreshold = 15,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetProfitabilityQuery(from, to, groupBy, marginAlertThreshold),
            cancellationToken);

        return Ok(new ProfitabilityResponse(
            result.PeriodStart,
            result.PeriodEnd,
            result.GroupBy.ToString(),
            result.MarginAlertThresholdPercent,
            result.Groups.Select(g => new ProfitabilityGroupResponse(
                g.GroupKey,
                g.ClientId,
                g.ProjectCount,
                g.TotalCost,
                g.TotalRevenue,
                g.TotalMargin,
                g.MarginPercent,
                g.IsLowMarginAlert)).ToList()));
    }

    [HttpGet("overview")]
    [ProducesResponseType(typeof(FinancialOverviewResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<FinancialOverviewResponse>> GetOverview(
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        [FromQuery] decimal marginAlertThreshold = 15,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetFinancialOverviewQuery(from, to, marginAlertThreshold),
            cancellationToken);

        return Ok(new FinancialOverviewResponse(
            result.PeriodStart,
            result.PeriodEnd,
            result.MarginAlertThresholdPercent,
            result.TotalCost,
            result.TotalRevenue,
            result.TotalMargin,
            result.AvgMarginPercent,
            result.Projects.Select(p => new ProjectFinancialSummaryResponse(
                p.ProjectId,
                p.ProjectName,
                p.ClientName,
                p.Status,
                p.EstimatedRevenue,
                p.TotalCost,
                p.MarginAmount,
                p.MarginPercent,
                p.IsLowMarginAlert)).ToList(),
            result.LowMarginAlerts.Select(a => new LowMarginAlertResponse(
                a.ProjectId,
                a.ProjectName,
                a.ClientName,
                a.MarginPercent,
                a.TotalCost,
                a.EstimatedRevenue)).ToList()));
    }
}
