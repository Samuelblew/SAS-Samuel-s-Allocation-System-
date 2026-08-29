using IAS.Api.Dtos;
using IAS.Application.Clients;
using IAS.Application.Clients.Commands.CreateClient;
using IAS.Application.Clients.Commands.DeleteClient;
using IAS.Application.Clients.Commands.UpdateClient;
using IAS.Application.Clients.Queries.GetClientById;
using IAS.Application.Clients.Queries.ListClients;
using IAS.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IAS.Api.Controllers;

[ApiController]
[Route("api/v1/clients")]
public sealed class ClientsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedClientsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedClientsResponse>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new ListClientsQuery(page, pageSize), cancellationToken);
        return Ok(MapPaged(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var client = await mediator.Send(new GetClientByIdQuery(id), cancellationToken);
        return Ok(Map(client));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClientResponse>> Create(
        [FromBody] CreateClientRequest request,
        CancellationToken cancellationToken)
    {
        var client = await mediator.Send(
            new CreateClientCommand(request.Name, request.Notes),
            cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = client.Id }, Map(client));
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ClientResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ClientResponse>> Update(
        Guid id,
        [FromBody] UpdateClientRequest request,
        CancellationToken cancellationToken)
    {
        var client = await mediator.Send(
            new UpdateClientCommand(id, request.Name, request.Notes),
            cancellationToken);

        return Ok(Map(client));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteClientCommand(id), cancellationToken);
        return NoContent();
    }

    private static ClientResponse Map(ClientDto dto) =>
        new(dto.Id, dto.Name, dto.Notes, dto.CreatedAt, dto.UpdatedAt);

    private static ClientListItemResponse MapListItem(ClientListItemDto dto) =>
        new(dto.Id, dto.Name, dto.ProjectCount, dto.CreatedAt);

    private static PagedClientsResponse MapPaged(PagedResult<ClientListItemDto> result) =>
        new(
            result.Items.Select(MapListItem).ToList(),
            result.Page,
            result.PageSize,
            result.TotalCount,
            result.TotalPages);
}
