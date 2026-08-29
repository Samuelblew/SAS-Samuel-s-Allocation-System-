using IAS.Api.Dtos;
using IAS.Application.Capacity.Queries.GetCapacityOverview;
using IAS.Application.Capacity.Queries.GetPersonAvailability;
using IAS.Application.Capacity.Queries.GetProjectStaffingGaps;
using IAS.Application.Capacity.Queries.ListAvailablePeople;
using IAS.Application.Capacity.Queries.ListBenchPeople;
using IAS.Application.Capacity.Queries.GetFutureCapacityGaps;
using IAS.Application.Capacity.Queries.GetSkillsOccupation;
using IAS.Application.Capacity.Queries.ListUnderstaffedProjects;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IAS.Api.Controllers;

[ApiController]
[Route("api/v1/capacity")]
public sealed class CapacityController(IMediator mediator) : ControllerBase
{
    [HttpGet("overview")]
    [ProducesResponseType(typeof(CapacityOverviewResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<CapacityOverviewResponse>> GetOverview(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        [FromQuery] decimal benchThreshold = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetCapacityOverviewQuery(from, to, benchThreshold),
            cancellationToken);

        return Ok(new CapacityOverviewResponse(
            result.From,
            result.To,
            result.Weeks.Select(w => new WeekOverviewResponse(
                w.WeekStart, w.WeekEnd, w.ActivePeopleCount,
                w.AvgAllocatedPercent, w.AvgAvailablePercent,
                w.BenchPeopleCount, w.OverallocatedPeopleCount,
                w.TotalCapacityHours, w.TotalAllocatedHours, w.TotalAvailableHours)).ToList(),
            result.Teams.Select(t => new TeamOccupationResponse(
                t.Team, t.PeopleCount, t.AvgAllocatedPercent, t.AvgAvailablePercent)).ToList()));
    }

    [HttpGet("people/{personId:guid}/availability")]
    [ProducesResponseType(typeof(PersonAvailabilityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonAvailabilityResponse>> GetPersonAvailability(
        Guid personId,
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new GetPersonAvailabilityQuery(personId, from, to),
            cancellationToken);

        return Ok(new PersonAvailabilityResponse(
            result.PersonId,
            result.PersonName,
            result.WeeklyCapacityHours,
            result.From,
            result.To,
            result.Weeks.Select(w => new WeekAvailabilityResponse(
                w.WeekStart,
                w.WeekEnd,
                w.AllocatedPercent,
                w.AvailablePercent,
                w.WeeklyCapacityHours,
                w.AllocatedHours,
                w.AvailableHours,
                w.IsUnavailable)).ToList()));
    }

    [HttpGet("projects/{projectId:guid}/staffing-gaps")]
    [ProducesResponseType(typeof(ProjectStaffingGapsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectStaffingGapsResponse>> GetProjectStaffingGaps(
        Guid projectId,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProjectStaffingGapsQuery(projectId), cancellationToken);

        return Ok(new ProjectStaffingGapsResponse(
            result.ProjectId,
            result.ProjectName,
            result.Needs.Select(n => new StaffingGapItemResponse(
                n.NeedId, n.Role, n.RequiredPercent, n.CoveredPercent, n.GapPercent, n.Status)).ToList()));
    }

    [HttpGet("people-available")]
    [ProducesResponseType(typeof(AvailablePeopleResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AvailablePeopleResponse>> ListAvailablePeople(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        [FromQuery] decimal minAvailablePercent = 1,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new ListAvailablePeopleQuery(from, to, minAvailablePercent),
            cancellationToken);

        return Ok(new AvailablePeopleResponse(
            result.From,
            result.To,
            result.People.Select(p => new AvailablePersonResponse(
                p.PersonId, p.PersonName, p.MinAvailablePercentInPeriod, p.AvgAvailablePercent)).ToList()));
    }

    [HttpGet("bench")]
    [ProducesResponseType(typeof(BenchPeopleResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<BenchPeopleResponse>> ListBench(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        [FromQuery] decimal minAvailablePercent = 50,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new ListBenchPeopleQuery(from, to, minAvailablePercent),
            cancellationToken);

        return Ok(new BenchPeopleResponse(
            result.From,
            result.To,
            result.MinAvailablePercent,
            result.People.Select(p => new BenchPersonResponse(
                p.PersonId, p.PersonName, p.Team, p.Seniority,
                p.MinAvailablePercentInPeriod, p.AvgAvailablePercent)).ToList()));
    }

    [HttpGet("future-gaps")]
    [ProducesResponseType(typeof(FutureCapacityGapsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<FutureCapacityGapsResponse>> GetFutureGaps(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetFutureCapacityGapsQuery(from, to), cancellationToken);

        return Ok(new FutureCapacityGapsResponse(
            result.From,
            result.To,
            result.PeakShortfallPercent,
            result.Weeks.Select(w => new WeekCapacityGapResponse(
                w.WeekStart,
                w.WeekEnd,
                w.TotalGapDemandPercent,
                w.TotalAvailableSupplyPercent,
                w.NetShortfallPercent,
                w.OpenNeedsInWeek)).ToList(),
            result.OpenNeeds.Select(n => new OpenNeedGapResponse(
                n.NeedId,
                n.ProjectId,
                n.ProjectName,
                n.Role,
                n.RequiredPercent,
                n.CoveredPercent,
                n.GapPercent,
                n.Status,
                n.StartDate,
                n.EndDate)).ToList()));
    }

    [HttpGet("skills-occupation")]
    [ProducesResponseType(typeof(SkillsOccupationResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<SkillsOccupationResponse>> GetSkillsOccupation(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetSkillsOccupationQuery(from, to), cancellationToken);

        return Ok(new SkillsOccupationResponse(
            result.From,
            result.To,
            result.Skills.Select(s => new SkillOccupationResponse(
                s.SkillId,
                s.SkillName,
                s.Category,
                s.PeopleCount,
                s.AvgAllocatedPercent,
                s.AvgAvailablePercent,
                s.AvgAllocatedHours,
                s.AvgAvailableHours)).ToList()));
    }

    [HttpGet("projects-understaffed")]
    [ProducesResponseType(typeof(UnderstaffedProjectsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UnderstaffedProjectsResponse>> ListUnderstaffedProjects(
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ListUnderstaffedProjectsQuery(), cancellationToken);

        return Ok(new UnderstaffedProjectsResponse(
            result.Items.Select(p => new UnderstaffedProjectResponse(
                p.ProjectId, p.ProjectName, p.Status, p.OpenNeedsCount, p.TotalGapPercent)).ToList()));
    }
}
