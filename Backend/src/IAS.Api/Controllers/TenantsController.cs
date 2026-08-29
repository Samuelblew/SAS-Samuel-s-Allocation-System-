using IAS.Api.Dtos;
using IAS.Application.Tenancy;
using IAS.Application.Tenancy.Commands.CreateTenant;
using IAS.Application.Tenancy.Queries.GetTenantById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IAS.Api.Controllers;

[ApiController]
[Route("api/v1/tenants")]
public sealed class TenantsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(TenantResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<TenantResponse>> Create(
        [FromBody] CreateTenantRequest request,
        CancellationToken cancellationToken)
    {
        var tenant = await mediator.Send(new CreateTenantCommand(request.Name), cancellationToken);
        var response = Map(tenant);
        return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TenantResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TenantResponse>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var tenant = await mediator.Send(new GetTenantByIdQuery(id), cancellationToken);
        return Ok(Map(tenant));
    }

    private static TenantResponse Map(TenantDto dto) =>
        new(dto.Id, dto.Name, dto.IsActive, dto.CreatedAt, dto.UpdatedAt);
}
