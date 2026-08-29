using FluentValidation;

namespace IAS.Application.AllocationNeeds.Queries.ListAllocationNeeds;

public sealed class ListAllocationNeedsQueryValidator : AbstractValidator<ListAllocationNeedsQuery>
{
    public ListAllocationNeedsQueryValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
