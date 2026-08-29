using FluentValidation;

namespace IAS.Application.Allocations.Queries.ListAllocationConflicts;

public sealed class ListAllocationConflictsQueryValidator : AbstractValidator<ListAllocationConflictsQuery>
{
    public ListAllocationConflictsQueryValidator()
    {
        RuleFor(x => x.To).GreaterThanOrEqualTo(x => x.From)
            .When(x => x.From.HasValue && x.To.HasValue);
    }
}
