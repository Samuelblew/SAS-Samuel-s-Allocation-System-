using IAS.Api.Auth;
using IAS.Api.Dtos;
using IAS.Application.Common.Exceptions;
using IAS.Application.Identity.Queries.GetUserById;
using IAS.Application.Tenancy.Queries.GetTenantById;
using IAS.Infrastructure.Tenancy;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IAS.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController(
    IMediator mediator,
    DevJwtTokenService tokenService,
    TenantContext tenantContext,
    IHostEnvironment environment) : ControllerBase
{
    [HttpPost("dev-token")]
    [ProducesResponseType(typeof(DevTokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DevTokenResponse>> CreateDevToken(
        [FromBody] DevTokenRequest request,
        CancellationToken cancellationToken)
    {
        if (!environment.IsDevelopment() && !environment.IsEnvironment("Testing"))
            return NotFound();

        try
        {
            await mediator.Send(new GetTenantByIdQuery(request.TenantId), cancellationToken);

            tenantContext.SetTenant(request.TenantId);
            var user = await mediator.Send(new GetUserByIdQuery(request.UserId), cancellationToken);

            var hours = int.TryParse(HttpContext.RequestServices
                .GetRequiredService<IConfiguration>()["Auth:Jwt:ExpirationHours"], out var h) ? h : 8;

            var token = tokenService.CreateToken(
                request.TenantId, request.UserId, user.Email, user.DisplayName);

            return Ok(new DevTokenResponse(token, "Bearer", hours));
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }
}
