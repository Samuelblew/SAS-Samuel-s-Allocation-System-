using IAS.Api.Dtos;
using IAS.Application.Allocations;
using IAS.Application.Common.Models;
using IAS.Application.Projects;
using IAS.Application.Projects.Commands.CreateProject;
using IAS.Application.Projects.Commands.DeleteProject;
using IAS.Application.Projects.Commands.UpdateProject;
using IAS.Application.Allocations.Queries.GetProjectPeopleView;
using IAS.Application.Projects.Queries.GetProjectById;
using IAS.Application.Capacity.Queries.ListUnderstaffedProjects;
using IAS.Application.Financial.Queries.GetProjectFinancials;
using IAS.Application.Matching.Queries.GetProjectMatchingCandidates;
using IAS.Application.Projects.Queries.ListProjects;
using IAS.Api.Mapping;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IAS.Api.Controllers;

[ApiController]
[Route("api/v1/projects")]
public sealed class ProjectsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedProjectsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedProjectsResponse>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? clientId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new ListProjectsQuery(page, pageSize, clientId),
            cancellationToken);

        return Ok(MapPaged(result));
    }

    [HttpGet("understaffed")]
    [ProducesResponseType(typeof(UnderstaffedProjectsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<UnderstaffedProjectsResponse>> ListUnderstaffed(
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ListUnderstaffedProjectsQuery(), cancellationToken);

        return Ok(new UnderstaffedProjectsResponse(
            result.Items.Select(p => new UnderstaffedProjectResponse(
                p.ProjectId, p.ProjectName, p.Status, p.OpenNeedsCount, p.TotalGapPercent)).ToList()));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var project = await mediator.Send(new GetProjectByIdQuery(id), cancellationToken);
        return Ok(Map(project));
    }

    [HttpGet("{id:guid}/matching-candidates")]
    [ProducesResponseType(typeof(ProjectMatchingCandidatesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectMatchingCandidatesResponse>> GetMatchingCandidates(
        Guid id,
        [FromQuery] int maxResultsPerNeed = 10,
        [FromQuery] decimal? minAvailablePercent = null,
        [FromQuery] bool excludePeopleOnProject = false,
        [FromQuery] bool openNeedsOnly = true,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetProjectMatchingCandidatesQuery(
                id,
                maxResultsPerNeed,
                minAvailablePercent,
                excludePeopleOnProject,
                openNeedsOnly),
            cancellationToken);

        return Ok(MatchingResponseMapper.MapProject(result));
    }

    [HttpGet("{id:guid}/people")]
    [ProducesResponseType(typeof(ProjectPeopleViewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectPeopleViewResponse>> GetPeopleView(
        Guid id,
        CancellationToken cancellationToken)
    {
        var view = await mediator.Send(new GetProjectPeopleViewQuery(id), cancellationToken);
        return Ok(MapPeopleView(view));
    }

    [HttpGet("{id:guid}/financials")]
    [ProducesResponseType(typeof(ProjectFinancialsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectFinancialsResponse>> GetFinancials(
        Guid id,
        [FromQuery] DateOnly? from,
        [FromQuery] DateOnly? to,
        [FromQuery] decimal marginAlertThreshold = 15,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetProjectFinancialsQuery(id, from, to, marginAlertThreshold),
            cancellationToken);

        return Ok(new ProjectFinancialsResponse(
            result.ProjectId,
            result.ProjectName,
            result.ClientId,
            result.ClientName,
            result.Status,
            result.PeriodStart,
            result.PeriodEnd,
            result.EstimatedRevenue,
            result.Budget,
            result.TotalCost,
            result.MarginAmount,
            result.MarginPercent,
            result.HasRevenueData,
            result.HasCostData,
            result.IsLowMarginAlert,
            result.MarginAlertThresholdPercent,
            result.Allocations.Select(a => new AllocationCostResponse(
                a.AllocationId,
                a.PersonId,
                a.PersonName,
                a.Role,
                a.DedicationPercent,
                a.AllocationStart,
                a.AllocationEnd,
                a.HourlyRate,
                a.WeeksInPeriod,
                a.TotalHours,
                a.TotalCost,
                a.HasCostData)).ToList()));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectResponse>> Create(
        [FromBody] CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var project = await mediator.Send(
            new CreateProjectCommand(
                request.ClientId,
                request.Name,
                request.Status,
                request.StartDate,
                request.EndDate,
                request.Priority,
                request.Budget,
                request.EstimatedRevenue,
                request.ProjectType,
                request.CommercialOwner,
                request.DeliveryOwner),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = project.Id }, Map(project));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectResponse>> Update(
        Guid id,
        [FromBody] UpdateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var project = await mediator.Send(
            new UpdateProjectCommand(
                id,
                request.ClientId,
                request.Name,
                request.Status,
                request.StartDate,
                request.EndDate,
                request.Priority,
                request.Budget,
                request.EstimatedRevenue,
                request.ProjectType,
                request.CommercialOwner,
                request.DeliveryOwner),
            cancellationToken);

        return Ok(Map(project));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteProjectCommand(id), cancellationToken);
        return NoContent();
    }

    private static ProjectResponse Map(ProjectDto dto) =>
        new(
            dto.Id,
            dto.ClientId,
            dto.ClientName,
            dto.Name,
            dto.Status,
            dto.StartDate,
            dto.EndDate,
            dto.Priority,
            dto.Budget,
            dto.EstimatedRevenue,
            dto.ProjectType,
            dto.CommercialOwner,
            dto.DeliveryOwner,
            dto.CreatedAt,
            dto.UpdatedAt);

    private static ProjectListItemResponse MapListItem(ProjectListItemDto dto) =>
        new(
            dto.Id,
            dto.ClientId,
            dto.ClientName,
            dto.Name,
            dto.Status,
            dto.Priority,
            dto.StartDate,
            dto.EndDate,
            dto.CreatedAt);

    private static PagedProjectsResponse MapPaged(PagedResult<ProjectListItemDto> result) =>
        new(
            result.Items.Select(MapListItem).ToList(),
            result.Page,
            result.PageSize,
            result.TotalCount,
            result.TotalPages);

    private static ProjectPeopleViewResponse MapPeopleView(ProjectPeopleViewDto dto) =>
        new(
            dto.ProjectId,
            dto.ProjectName,
            dto.People.Select(p => new ProjectPersonEntryResponse(
                p.PersonId,
                p.PersonName,
                p.JobTitle,
                p.Status,
                p.Allocations.Select(MapViewItem).ToList())).ToList());

    private static AllocationViewItemResponse MapViewItem(AllocationViewItemDto dto) =>
        new(dto.Id, dto.Role, dto.DedicationPercent, dto.StartDate, dto.EndDate, dto.Status, dto.Notes);
}
