using IAS.Api.Dtos;
using IAS.Application.AuditLogs;
using IAS.Application.AuditLogs.Queries.ListAuditLogs;
using IAS.Application.Common.Models;
using IAS.Domain.AuditLogs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IAS.Api.Controllers;

[ApiController]
[Route("api/v1/audit-logs")]
public sealed class AuditLogsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedAuditLogsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedAuditLogsResponse>> List(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? entityType = null,
        [FromQuery] Guid? entityId = null,
        [FromQuery] AuditAction? action = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(
            new ListAuditLogsQuery(page, pageSize, entityType, entityId, action, from, to),
            cancellationToken);

        return Ok(MapPaged(result));
    }

    private static AuditLogResponse Map(AuditLogDto dto) =>
        new(
            dto.Id,
            dto.EntityType,
            dto.EntityId,
            dto.Action,
            dto.ActorId,
            dto.Summary,
            dto.ChangesJson,
            dto.OccurredAt);

    private static PagedAuditLogsResponse MapPaged(PagedResult<AuditLogDto> result) =>
        new(
            result.Items.Select(Map).ToList(),
            result.Page,
            result.PageSize,
            result.TotalCount,
            result.TotalPages);
}
