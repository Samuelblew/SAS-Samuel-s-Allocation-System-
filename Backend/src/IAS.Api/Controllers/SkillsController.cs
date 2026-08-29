using IAS.Api.Dtos;
using IAS.Application.Common.Models;
using IAS.Application.Skills;
using IAS.Application.Skills.Commands.CreateSkill;
using IAS.Application.Skills.Commands.DeleteSkill;
using IAS.Application.Skills.Commands.UpdateSkill;
using IAS.Application.Skills.Queries.GetSkillById;
using IAS.Application.Skills.Queries.ListSkills;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IAS.Api.Controllers;

[ApiController]
[Route("api/v1/skills")]
public sealed class SkillsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedSkillsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedSkillsResponse>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new ListSkillsQuery(page, pageSize), cancellationToken);
        return Ok(MapPaged(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SkillResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SkillResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var skill = await mediator.Send(new GetSkillByIdQuery(id), cancellationToken);
        return Ok(Map(skill));
    }

    [HttpPost]
    [ProducesResponseType(typeof(SkillResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SkillResponse>> Create(
        [FromBody] CreateSkillRequest request,
        CancellationToken cancellationToken)
    {
        var skill = await mediator.Send(
            new CreateSkillCommand(request.Name, request.Category),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = skill.Id }, Map(skill));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(SkillResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<SkillResponse>> Update(
        Guid id,
        [FromBody] UpdateSkillRequest request,
        CancellationToken cancellationToken)
    {
        var skill = await mediator.Send(
            new UpdateSkillCommand(id, request.Name, request.Category),
            cancellationToken);

        return Ok(Map(skill));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteSkillCommand(id), cancellationToken);
        return NoContent();
    }

    private static SkillResponse Map(SkillDto dto) =>
        new(dto.Id, dto.Name, dto.Category, dto.CreatedAt, dto.UpdatedAt);

    private static PagedSkillsResponse MapPaged(PagedResult<SkillDto> result) =>
        new(
            result.Items.Select(Map).ToList(),
            result.Page,
            result.PageSize,
            result.TotalCount,
            result.TotalPages);
}
