using FluentValidation;
using IAS.Domain.Matching;

namespace IAS.Application.Matching.Commands.RecordMatchingSuggestion;

public sealed class RecordMatchingSuggestionCommandValidator : AbstractValidator<RecordMatchingSuggestionCommand>
{
    public RecordMatchingSuggestionCommandValidator()
    {
        RuleFor(x => x.AllocationNeedId).NotEmpty();
        RuleFor(x => x.PersonId).NotEmpty();
        RuleFor(x => x.Decision).IsInEnum();
        RuleFor(x => x.Score).InclusiveBetween(-50, 100);
        RuleFor(x => x.Notes).MaximumLength(500).When(x => x.Notes != null);
    }
}
