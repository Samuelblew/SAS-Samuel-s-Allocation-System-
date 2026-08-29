using FluentValidation;

namespace IAS.Application.Matching.Queries.ListMatchingSuggestions;

public sealed class ListMatchingSuggestionsQueryValidator : AbstractValidator<ListMatchingSuggestionsQuery>
{
    public ListMatchingSuggestionsQueryValidator()
    {
        RuleFor(x => x.AllocationNeedId).NotEmpty();
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
