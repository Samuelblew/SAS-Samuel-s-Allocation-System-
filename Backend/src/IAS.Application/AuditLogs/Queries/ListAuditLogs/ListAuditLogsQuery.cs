using IAS.Application.Common.Models;
using IAS.Domain.AuditLogs;
using MediatR;

namespace IAS.Application.AuditLogs.Queries.ListAuditLogs;

public sealed record ListAuditLogsQuery(
    int Page = 1,
    int PageSize = 20,
    string? EntityType = null,
    Guid? EntityId = null,
    AuditAction? Action = null,
    DateTime? From = null,
    DateTime? To = null) : IRequest<PagedResult<AuditLogDto>>;
