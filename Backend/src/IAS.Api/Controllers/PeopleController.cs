using IAS.Api.Dtos;
using IAS.Application.Common.Models;
using IAS.Application.People;
using IAS.Application.People.Commands.AddPersonSkill;
using IAS.Application.People.Commands.CreatePerson;
using IAS.Application.People.Commands.DeletePerson;
using IAS.Application.People.Commands.RemovePersonSkill;
using IAS.Application.People.Commands.UpdatePerson;
using IAS.Application.People.Commands.UpdatePersonSkill;
using IAS.Application.Allocations;
using IAS.Application.Allocations.Queries.GetPersonProjectsView;
using IAS.Application.People.Queries.GetPersonById;
using IAS.Application.People.Queries.ListPeople;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IAS.Api.Controllers;

[ApiController]
[Route("api/v1/people")]
public sealed class PeopleController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedPeopleResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedPeopleResponse>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new ListPeopleQuery(page, pageSize), cancellationToken);
        return Ok(MapPaged(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var person = await mediator.Send(new GetPersonByIdQuery(id), cancellationToken);
        return Ok(Map(person));
    }

    [HttpGet("{id:guid}/projects")]
    [ProducesResponseType(typeof(PersonProjectsViewResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonProjectsViewResponse>> GetProjectsView(
        Guid id,
        CancellationToken cancellationToken)
    {
        var view = await mediator.Send(new GetPersonProjectsViewQuery(id), cancellationToken);
        return Ok(MapProjectsView(view));
    }

    [HttpPost]
    [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<PersonResponse>> Create(
        [FromBody] CreatePersonRequest request,
        CancellationToken cancellationToken)
    {
        var person = await mediator.Send(
            new CreatePersonCommand(
                request.Name,
                request.JobTitle,
                request.Seniority,
                request.HourlyCost,
                request.MonthlyCost,
                request.WeeklyCapacityHours,
                request.Location,
                request.Team,
                request.Status),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = person.Id }, Map(person));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonResponse>> Update(
        Guid id,
        [FromBody] UpdatePersonRequest request,
        CancellationToken cancellationToken)
    {
        var person = await mediator.Send(
            new UpdatePersonCommand(
                id,
                request.Name,
                request.JobTitle,
                request.Seniority,
                request.HourlyCost,
                request.MonthlyCost,
                request.WeeklyCapacityHours,
                request.Location,
                request.Team,
                request.Status),
            cancellationToken);

        return Ok(Map(person));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeletePersonCommand(id), cancellationToken);
        return NoContent();
    }

    [HttpPost("{personId:guid}/skills")]
    [ProducesResponseType(typeof(PersonSkillResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PersonSkillResponse>> AddSkill(
        Guid personId,
        [FromBody] AddPersonSkillRequest request,
        CancellationToken cancellationToken)
    {
        var skill = await mediator.Send(
            new AddPersonSkillCommand(
                personId,
                request.SkillId,
                request.Level,
                request.LastUsedAt,
                request.Notes),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = personId }, MapSkill(skill));
    }

    [HttpPut("{personId:guid}/skills/{personSkillId:guid}")]
    [ProducesResponseType(typeof(PersonSkillResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonSkillResponse>> UpdateSkill(
        Guid personId,
        Guid personSkillId,
        [FromBody] UpdatePersonSkillRequest request,
        CancellationToken cancellationToken)
    {
        var skill = await mediator.Send(
            new UpdatePersonSkillCommand(
                personId,
                personSkillId,
                request.Level,
                request.LastUsedAt,
                request.Notes),
            cancellationToken);

        return Ok(MapSkill(skill));
    }

    [HttpDelete("{personId:guid}/skills/{personSkillId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveSkill(
        Guid personId,
        Guid personSkillId,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new RemovePersonSkillCommand(personId, personSkillId), cancellationToken);
        return NoContent();
    }

    private static PersonResponse Map(PersonDto dto) =>
        new(
            dto.Id,
            dto.Name,
            dto.JobTitle,
            dto.Seniority,
            dto.HourlyCost,
            dto.MonthlyCost,
            dto.WeeklyCapacityHours,
            dto.Location,
            dto.Team,
            dto.Status,
            dto.Skills.Select(MapSkill).ToList(),
            dto.CreatedAt,
            dto.UpdatedAt);

    private static PersonListItemResponse MapListItem(PersonListItemDto dto) =>
        new(
            dto.Id,
            dto.Name,
            dto.JobTitle,
            dto.Seniority,
            dto.WeeklyCapacityHours,
            dto.Status,
            dto.SkillCount,
            dto.CreatedAt);

    private static PersonSkillResponse MapSkill(PersonSkillDto dto) =>
        new(dto.Id, dto.SkillId, dto.SkillName, dto.SkillCategory, dto.Level, dto.LastUsedAt, dto.Notes);

    private static PagedPeopleResponse MapPaged(PagedResult<PersonListItemDto> result) =>
        new(
            result.Items.Select(MapListItem).ToList(),
            result.Page,
            result.PageSize,
            result.TotalCount,
            result.TotalPages);

    private static PersonProjectsViewResponse MapProjectsView(PersonProjectsViewDto dto) =>
        new(
            dto.PersonId,
            dto.PersonName,
            dto.Projects.Select(p => new PersonProjectEntryResponse(
                p.ProjectId,
                p.ProjectName,
                p.ProjectStatus,
                p.Allocations.Select(MapViewItem).ToList())).ToList());

    private static AllocationViewItemResponse MapViewItem(AllocationViewItemDto dto) =>
        new(dto.Id, dto.Role, dto.DedicationPercent, dto.StartDate, dto.EndDate, dto.Status, dto.Notes);
}
