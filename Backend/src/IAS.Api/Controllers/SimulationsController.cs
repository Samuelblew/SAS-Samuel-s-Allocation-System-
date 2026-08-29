using IAS.Api.Dtos;
using IAS.Application.Capacity.Commands.SimulateProjectFeasibility;
using IAS.Application.Financial.Commands.SimulateAllocationMargin;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IAS.Api.Controllers;

[ApiController]
[Route("api/v1/simulations")]
public sealed class SimulationsController(IMediator mediator) : ControllerBase
{
    [HttpPost("project-feasibility")]
    [ProducesResponseType(typeof(ProjectFeasibilityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProjectFeasibilityResponse>> SimulateProjectFeasibility(
        [FromBody] SimulateProjectFeasibilityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new SimulateProjectFeasibilityCommand(
                request.DesiredStartDate,
                request.DurationMonths,
                request.Needs.Select(n => new SimulatedNeedInput(
                    n.Role,
                    n.ExpectedSeniority,
                    n.RequiredSkillIds,
                    n.DedicationPercent,
                    n.Quantity)).ToList()),
            cancellationToken);

        return Ok(new ProjectFeasibilityResponse(
            result.DesiredStartDate,
            result.SimulatedEndDate,
            result.FeasibleAtDesiredStart,
            result.EarliestFeasibleStart,
            result.WeeksScanned,
            result.ActivePeopleCount,
            result.BenchAtDesiredStart,
            result.TotalHeadcountRequired,
            result.Roles.Select(r => new RoleFeasibilityResponse(
                r.Role,
                r.ExpectedSeniority,
                r.DedicationPercent,
                r.QuantityRequired,
                r.CandidatesAtDesiredStart,
                r.SatisfiedAtDesiredStart,
                r.EligibleCandidates.Select(c => new RoleCandidatePreviewResponse(
                    c.PersonId,
                    c.PersonName,
                    c.Seniority,
                    c.MinAvailablePercent)).ToList())).ToList()));
    }

    [HttpPost("allocation-margin")]
    [ProducesResponseType(typeof(AllocationMarginSimulationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AllocationMarginSimulationResponse>> SimulateAllocationMargin(
        [FromBody] SimulateAllocationMarginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new SimulateAllocationMarginCommand(
                request.ProjectId,
                request.PersonId,
                request.Role,
                request.DedicationPercent,
                request.StartDate,
                request.EndDate,
                request.MarginAlertThresholdPercent),
            cancellationToken);

        return Ok(new AllocationMarginSimulationResponse(
            result.ProjectId,
            result.ProjectName,
            result.PeriodStart,
            result.PeriodEnd,
            result.CurrentTotalCost,
            result.CurrentMarginAmount,
            result.CurrentMarginPercent,
            result.SimulatedAdditionalCost,
            result.ProjectedTotalCost,
            result.ProjectedMarginAmount,
            result.ProjectedMarginPercent,
            result.MarginDeltaAmount,
            result.MarginDeltaPercent,
            result.HasRevenueData,
            result.CurrentIsLowMarginAlert,
            result.ProjectedIsLowMarginAlert,
            result.MarginAlertThresholdPercent));
    }
}
