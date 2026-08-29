using IAS.Application.Matching.Queries.GetAllocationNeedCandidates;
using MediatR;

namespace IAS.Application.Matching.Queries.GetProjectMatchingCandidates;

public sealed record GetProjectMatchingCandidatesQuery(
    Guid ProjectId,
    int MaxResultsPerNeed = 10,
    decimal? MinAvailablePercent = null,
    bool ExcludePeopleOnProject = false,
    bool OpenNeedsOnly = true) : IRequest<ProjectMatchingCandidatesDto>;

public sealed record ProjectMatchingCandidatesDto(
    Guid ProjectId,
    string ProjectName,
    IReadOnlyList<AllocationNeedCandidatesDto> Needs);
