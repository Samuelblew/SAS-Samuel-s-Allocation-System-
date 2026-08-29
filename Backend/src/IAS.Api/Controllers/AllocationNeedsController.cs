using IAS.Api.Dtos;
using IAS.Application.AllocationNeeds;
using IAS.Application.AllocationNeeds.Commands.CreateAllocationNeed;
using IAS.Application.AllocationNeeds.Commands.DeleteAllocationNeed;
using IAS.Application.AllocationNeeds.Commands.UpdateAllocationNeed;
using IAS.Application.AllocationNeeds.Queries.GetAllocationNeedById;
using IAS.Application.AllocationNeeds.Queries.ListAllocationNeeds;
using IAS.Application.Common.Models;
using IAS.Api.Mapping;
using IAS.Application.Matching.Commands.RecordMatchingSuggestion;
using IAS.Application.Matching.Queries.GetAllocationNeedCandidates;
using IAS.Application.Matching.Queries.ListMatchingSuggestions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IAS.Api.Controllers;

[ApiController]
[Route("api/v1/allocation-needs")]
public sealed class AllocationNeedsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedAllocationNeedsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedAllocationNeedsResponse>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? projectId = null,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new ListAllocationNeedsQuery(page, pageSize, projectId),
            cancellationToken);

        return Ok(MapPaged(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(AllocationNeedResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AllocationNeedResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var need = await mediator.Send(new GetAllocationNeedByIdQuery(id), cancellationToken);
        return Ok(Map(need));
    }

    [HttpGet("{id:guid}/candidates")]
    [ProducesResponseType(typeof(AllocationNeedCandidatesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AllocationNeedCandidatesResponse>> GetCandidates(
        Guid id,
        [FromQuery] int maxResults = 20,
        [FromQuery] decimal? minAvailablePercent = null,
        [FromQuery] bool excludePeopleOnProject = false,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new GetAllocationNeedCandidatesQuery(
                id,
                maxResults,
                minAvailablePercent,
                excludePeopleOnProject),
            cancellationToken);

        return Ok(MatchingResponseMapper.MapNeed(result));
    }

    [HttpGet("{id:guid}/matching-suggestions")]
    [ProducesResponseType(typeof(PagedMatchingSuggestionsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedMatchingSuggestionsResponse>> ListMatchingSuggestions(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new ListMatchingSuggestionsQuery(id, page, pageSize),
            cancellationToken);

        return Ok(new PagedMatchingSuggestionsResponse(
            result.Items.Select(MapSuggestion).ToList(),
            result.Page,
            result.PageSize,
            result.TotalCount,
            result.TotalPages));
    }

    [HttpPost("{id:guid}/matching-suggestions")]
    [ProducesResponseType(typeof(MatchingSuggestionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MatchingSuggestionResponse>> RecordMatchingSuggestion(
        Guid id,
        [FromBody] RecordMatchingSuggestionRequest request,
        CancellationToken cancellationToken)
    {
        var suggestion = await mediator.Send(
            new RecordMatchingSuggestionCommand(
                id,
                request.PersonId,
                request.Decision,
                request.Score,
                request.Notes),
            cancellationToken);

        return CreatedAtAction(
            nameof(ListMatchingSuggestions),
            new { id },
            MapSuggestion(suggestion));
    }

    [HttpPost]
    [ProducesResponseType(typeof(AllocationNeedResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AllocationNeedResponse>> Create(
        [FromBody] CreateAllocationNeedRequest request,
        CancellationToken cancellationToken)
    {
        var need = await mediator.Send(
            new CreateAllocationNeedCommand(
                request.ProjectId,
                request.Role,
                request.ExpectedSeniority,
                request.RequiredSkillIds,
                request.DesiredSkillIds,
                request.DedicationPercent,
                request.StartDate,
                request.EndDate,
                request.Urgency,
                request.Criticality,
                request.Status),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = need.Id }, Map(need));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(AllocationNeedResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AllocationNeedResponse>> Update(
        Guid id,
        [FromBody] UpdateAllocationNeedRequest request,
        CancellationToken cancellationToken)
    {
        var need = await mediator.Send(
            new UpdateAllocationNeedCommand(
                id,
                request.ProjectId,
                request.Role,
                request.ExpectedSeniority,
                request.RequiredSkillIds,
                request.DesiredSkillIds,
                request.DedicationPercent,
                request.StartDate,
                request.EndDate,
                request.Urgency,
                request.Criticality,
                request.Status),
            cancellationToken);

        return Ok(Map(need));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteAllocationNeedCommand(id), cancellationToken);
        return NoContent();
    }

    private static AllocationNeedResponse Map(AllocationNeedDto dto) =>
        new(
            dto.Id,
            dto.ProjectId,
            dto.ProjectName,
            dto.Role,
            dto.ExpectedSeniority,
            dto.RequiredSkillIds,
            dto.DesiredSkillIds,
            dto.DedicationPercent,
            dto.StartDate,
            dto.EndDate,
            dto.Urgency,
            dto.Criticality,
            dto.Status,
            dto.CreatedAt,
            dto.UpdatedAt);

    private static AllocationNeedListItemResponse MapListItem(AllocationNeedDto dto) =>
        new(
            dto.Id,
            dto.ProjectId,
            dto.ProjectName,
            dto.Role,
            dto.ExpectedSeniority,
            dto.RequiredSkillIds,
            dto.DedicationPercent,
            dto.StartDate,
            dto.EndDate,
            dto.Urgency,
            dto.Criticality,
            dto.Status,
            dto.CreatedAt);

    private static PagedAllocationNeedsResponse MapPaged(PagedResult<AllocationNeedDto> result) =>
        new(
            result.Items.Select(MapListItem).ToList(),
            result.Page,
            result.PageSize,
            result.TotalCount,
            result.TotalPages);

    private static MatchingSuggestionResponse MapSuggestion(MatchingSuggestionDto dto) =>
        new(
            dto.Id,
            dto.AllocationNeedId,
            dto.ProjectName,
            dto.NeedRole,
            dto.PersonId,
            dto.PersonName,
            dto.Decision,
            dto.Score,
            dto.Notes,
            dto.DecidedByUserId,
            dto.CreatedAt);
}
