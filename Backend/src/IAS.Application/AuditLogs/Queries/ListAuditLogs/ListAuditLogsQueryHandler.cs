using IAS.Application.AuditLogs;
using IAS.Application.Common.Models;
using MediatR;

namespace IAS.Application.AuditLogs.Queries.ListAuditLogs;

public sealed class ListAuditLogsQueryHandler(IAuditLogRepository repository)
    : IRequestHandler<ListAuditLogsQuery, PagedResult<AuditLogDto>>
{
    public async Task<PagedResult<AuditLogDto>> Handle(
        ListAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, total) = await repository.ListAsync(
            request.Page,
            request.PageSize,
            request.EntityType,
            request.EntityId,
            request.Action,
            request.From,
            request.To,
            cancellationToken);

        return new PagedResult<AuditLogDto>(
            items.Select(i => i.ToDto()).ToList(),
            request.Page,
            request.PageSize,
            total);
    }
}
