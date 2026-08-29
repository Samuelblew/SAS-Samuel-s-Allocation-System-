using FluentValidation;

namespace IAS.Application.Matching.Queries.GetProjectMatchingCandidates;

public sealed class GetProjectMatchingCandidatesQueryValidator
    : AbstractValidator<GetProjectMatchingCandidatesQuery>
{
    public GetProjectMatchingCandidatesQueryValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.MaxResultsPerNeed).InclusiveBetween(1, 50);
        RuleFor(x => x.MinAvailablePercent)
            .InclusiveBetween(0, 100)
            .When(x => x.MinAvailablePercent.HasValue);
    }
}
