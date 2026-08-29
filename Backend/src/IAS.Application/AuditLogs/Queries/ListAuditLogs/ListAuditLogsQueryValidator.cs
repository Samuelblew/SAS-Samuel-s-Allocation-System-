using FluentValidation;

namespace IAS.Application.AuditLogs.Queries.ListAuditLogs;

public sealed class ListAuditLogsQueryValidator : AbstractValidator<ListAuditLogsQuery>
{
    public ListAuditLogsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.EntityType).MaximumLength(80);
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From)
            .When(x => x.From.HasValue && x.To.HasValue);
        RuleFor(x => x.Action).IsInEnum().When(x => x.Action.HasValue);
    }
}
