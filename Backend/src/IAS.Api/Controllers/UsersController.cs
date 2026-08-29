using IAS.Api.Dtos;
using IAS.Application.Common.Models;
using IAS.Application.Identity;
using IAS.Application.Identity.Commands.CreateUser;
using IAS.Application.Identity.Commands.DeleteUser;
using IAS.Application.Identity.Commands.UpdateUser;
using IAS.Application.Identity.Queries.GetUserById;
using IAS.Application.Identity.Queries.ListUsers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IAS.Api.Controllers;

[ApiController]
[Route("api/v1/users")]
public sealed class UsersController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedUsersResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedUsersResponse>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new ListUsersQuery(page, pageSize), cancellationToken);
        return Ok(MapPaged(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await mediator.Send(new GetUserByIdQuery(id), cancellationToken);
        return Ok(Map(user));
    }

    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<UserResponse>> Create(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await mediator.Send(
            new CreateUserCommand(request.Email, request.DisplayName, request.Role),
            cancellationToken);

        var response = Map(user);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> Update(
        Guid id,
        [FromBody] UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await mediator.Send(
            new UpdateUserCommand(id, request.Email, request.DisplayName, request.Role, request.IsActive),
            cancellationToken);

        return Ok(Map(user));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteUserCommand(id), cancellationToken);
        return NoContent();
    }

    private static UserResponse Map(UserDto dto) =>
        new(dto.Id, dto.Email, dto.DisplayName, dto.Role, dto.IsActive, dto.CreatedAt, dto.UpdatedAt);

    private static PagedUsersResponse MapPaged(PagedResult<UserDto> result) =>
        new(
            result.Items.Select(Map).ToList(),
            result.Page,
            result.PageSize,
            result.TotalCount,
            result.TotalPages);
}
