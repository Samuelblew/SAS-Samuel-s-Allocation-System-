using FluentValidation;

namespace IAS.Application.Allocations.Queries.ListAllocations;

public sealed class ListAllocationsQueryValidator : AbstractValidator<ListAllocationsQuery>
{
    public ListAllocationsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
