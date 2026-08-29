using IAS.Api.Dtos;
using IAS.Application.Allocations;
using IAS.Application.Allocations.Commands.CreateAllocation;
using IAS.Application.Allocations.Commands.DeleteAllocation;
using IAS.Application.Allocations.Commands.UpdateAllocation;
using IAS.Application.Allocations.Queries.GetAllocationById;
using IAS.Application.Allocations.Queries.ListAllocationConflicts;
using IAS.Application.Allocations.Queries.ListAllocations;
using IAS.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IAS.Api.Controllers;

[ApiController]
[Route("api/v1/allocations")]
public sealed class AllocationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedAllocationsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedAllocationsResponse>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? personId = null,
        [FromQuery] Guid? projectId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new ListAllocationsQuery(page, pageSize, personId, projectId),
            cancellationToken);

        return Ok(MapPaged(result));
    }

    [HttpGet("conflicts")]
    [ProducesResponseType(typeof(AllocationConflictsListResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<AllocationConflictsListResponse>> ListConflicts(
        [FromQuery] Guid? personId = null,
        [FromQuery] Guid? projectId = null,
        [FromQuery] DateOnly? from = null,
        [FromQuery] DateOnly? to = null,
        CancellationToken cancellationToken = default)
    {
        var conflicts = await mediator.Send(
            new ListAllocationConflictsQuery(personId, projectId, from, to),
            cancellationToken);

        return Ok(new AllocationConflictsListResponse(conflicts.Select(MapConflict).ToList()));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AllocationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AllocationResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var allocation = await mediator.Send(new GetAllocationByIdQuery(id), cancellationToken);
        return Ok(Map(allocation));
    }

    [HttpPost]
    [ProducesResponseType(typeof(AllocationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AllocationResponse>> Create(
        [FromBody] CreateAllocationRequest request,
        CancellationToken cancellationToken)
    {
        var allocation = await mediator.Send(
            new CreateAllocationCommand(
                request.PersonId,
                request.ProjectId,
                request.Role,
                request.DedicationPercent,
                request.StartDate,
                request.EndDate,
                request.Status,
                request.Notes),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = allocation.Id }, Map(allocation));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AllocationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AllocationResponse>> Update(
        Guid id,
        [FromBody] UpdateAllocationRequest request,
        CancellationToken cancellationToken)
    {
        var allocation = await mediator.Send(
            new UpdateAllocationCommand(
                id,
                request.PersonId,
                request.ProjectId,
                request.Role,
                request.DedicationPercent,
                request.StartDate,
                request.EndDate,
                request.Status,
                request.Notes),
            cancellationToken);

        return Ok(Map(allocation));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteAllocationCommand(id), cancellationToken);
        return NoContent();
    }

    private static AllocationResponse Map(AllocationDto dto) =>
        new(
            dto.Id,
            dto.PersonId,
            dto.PersonName,
            dto.ProjectId,
            dto.ProjectName,
            dto.Role,
            dto.DedicationPercent,
            dto.StartDate,
            dto.EndDate,
            dto.Status,
            dto.Notes,
            dto.CreatedAt,
            dto.UpdatedAt);

    private static AllocationListItemResponse MapListItem(AllocationDto dto) =>
        new(
            dto.Id,
            dto.PersonId,
            dto.PersonName,
            dto.ProjectId,
            dto.ProjectName,
            dto.Role,
            dto.DedicationPercent,
            dto.Status,
            dto.StartDate,
            dto.EndDate,
            dto.CreatedAt);

    private static PagedAllocationsResponse MapPaged(PagedResult<AllocationDto> result) =>
        new(
            result.Items.Select(MapListItem).ToList(),
            result.Page,
            result.PageSize,
            result.TotalCount,
            result.TotalPages);

    private static AllocationConflictResponse MapConflict(AllocationConflictDto dto) =>
        new(
            dto.PersonId,
            dto.PersonName,
            dto.WeekStart,
            dto.WeekEnd,
            dto.TotalDedicationPercent,
            dto.Allocations.Select(a => new AllocationConflictItemResponse(
                a.AllocationId,
                a.ProjectId,
                a.ProjectName,
                a.DedicationPercent,
                a.StartDate,
                a.EndDate,
                a.Status)).ToList());
}
