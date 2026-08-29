using IAS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IAS.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class HealthController(IasDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

        if (!canConnect)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                status = "unhealthy",
                service = "IAS.Api",
                database = "disconnected"
            });
        }

        return Ok(new
        {
            status = "healthy",
            service = "IAS.Api",
            database = "connected"
        });
    }
}
