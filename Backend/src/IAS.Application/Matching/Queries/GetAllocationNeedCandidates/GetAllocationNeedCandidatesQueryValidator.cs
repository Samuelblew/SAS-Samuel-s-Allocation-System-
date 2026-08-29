using FluentValidation;

namespace IAS.Application.Matching.Queries.GetAllocationNeedCandidates;

public sealed class GetAllocationNeedCandidatesQueryValidator : AbstractValidator<GetAllocationNeedCandidatesQuery>
{
    public GetAllocationNeedCandidatesQueryValidator()
    {
        RuleFor(x => x.AllocationNeedId).NotEmpty();
        RuleFor(x => x.MaxResults).InclusiveBetween(1, 100);
        RuleFor(x => x.MinAvailablePercent)
            .InclusiveBetween(0, 100)
            .When(x => x.MinAvailablePercent.HasValue);
    }
}
