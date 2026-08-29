using IAS.Api.Dtos;
using IAS.Application.Common.Models;
using IAS.Application.Unavailabilities;
using IAS.Application.Unavailabilities.Commands.CreateUnavailability;
using IAS.Application.Unavailabilities.Commands.DeleteUnavailability;
using IAS.Application.Unavailabilities.Commands.UpdateUnavailability;
using IAS.Application.Unavailabilities.Queries.GetUnavailabilityById;
using IAS.Application.Unavailabilities.Queries.ListPersonUnavailabilities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IAS.Api.Controllers;

[ApiController]
[Route("api/v1/people/{personId:guid}/unavailabilities")]
public sealed class PersonUnavailabilitiesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedUnavailabilitiesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedUnavailabilitiesResponse>> List(
        Guid personId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new ListPersonUnavailabilitiesQuery(personId, page, pageSize),
            cancellationToken);

        return Ok(MapPaged(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UnavailabilityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UnavailabilityResponse>> GetById(
        Guid personId,
        Guid id,
        CancellationToken cancellationToken)
    {
        var item = await mediator.Send(new GetUnavailabilityByIdQuery(personId, id), cancellationToken);
        return Ok(Map(item));
    }

    [HttpPost]
    [ProducesResponseType(typeof(UnavailabilityResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UnavailabilityResponse>> Create(
        Guid personId,
        [FromBody] CreateUnavailabilityRequest request,
        CancellationToken cancellationToken)
    {
        var item = await mediator.Send(
            new CreateUnavailabilityCommand(
                personId,
                request.StartDate,
                request.EndDate,
                request.Type,
                request.Notes),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { personId, id = item.Id }, Map(item));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UnavailabilityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UnavailabilityResponse>> Update(
        Guid personId,
        Guid id,
        [FromBody] UpdateUnavailabilityRequest request,
        CancellationToken cancellationToken)
    {
        var item = await mediator.Send(
            new UpdateUnavailabilityCommand(
                personId,
                id,
                request.StartDate,
                request.EndDate,
                request.Type,
                request.Notes),
            cancellationToken);

        return Ok(Map(item));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid personId,
        Guid id,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteUnavailabilityCommand(personId, id), cancellationToken);
        return NoContent();
    }

    private static UnavailabilityResponse Map(UnavailabilityDto dto) =>
        new(dto.Id, dto.PersonId, dto.StartDate, dto.EndDate, dto.Type, dto.Notes, dto.CreatedAt, dto.UpdatedAt);

    private static PagedUnavailabilitiesResponse MapPaged(PagedResult<UnavailabilityDto> result) =>
        new(
            result.Items.Select(Map).ToList(),
            result.Page,
            result.PageSize,
            result.TotalCount,
            result.TotalPages);
}
