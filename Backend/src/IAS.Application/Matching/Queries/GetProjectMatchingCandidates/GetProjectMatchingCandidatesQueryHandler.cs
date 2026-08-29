using IAS.Application.AllocationNeeds;
using IAS.Application.Capacity;
using IAS.Application.Common.Exceptions;
using IAS.Application.Matching.Queries.GetAllocationNeedCandidates;
using IAS.Application.Projects;
using IAS.Domain.AllocationNeeds;
using MediatR;

namespace IAS.Application.Matching.Queries.GetProjectMatchingCandidates;

public sealed class GetProjectMatchingCandidatesQueryHandler(
    IProjectRepository projectRepository,
    IAllocationNeedRepository needRepository,
    ICapacityReadRepository capacityRepository)
    : IRequestHandler<GetProjectMatchingCandidatesQuery, ProjectMatchingCandidatesDto>
{
    public async Task<ProjectMatchingCandidatesDto> Handle(
        GetProjectMatchingCandidatesQuery request,
        CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetByIdAsync(request.ProjectId, cancellationToken)
            ?? throw new NotFoundException($"Projeto '{request.ProjectId}' não encontrado.");

        var needs = (await needRepository.ListByProjectAsync(request.ProjectId, cancellationToken)).ToList();

        if (request.OpenNeedsOnly)
            needs = needs.Where(n => n.Status != AllocationNeedStatus.Filled).ToList();

        if (needs.Count == 0)
        {
            return new ProjectMatchingCandidatesDto(project.Id, project.Name, []);
        }

        var periods = needs
            .Select(n => AllocationNeedCandidateMatcher.ResolvePeriod(n))
            .ToList();

        var periodStart = periods.Min(p => p.Start);
        var periodEnd = periods.Max(p => p.End);

        var data = await AllocationNeedCandidateRanker.LoadCapacityDataAsync(
            capacityRepository,
            periodStart,
            periodEnd,
            cancellationToken);

        var filters = new CandidateMatchFilters(
            request.MinAvailablePercent,
            request.ExcludePeopleOnProject);

        var rankedNeeds = needs
            .OrderBy(n => n.Role)
            .ThenBy(n => n.CreatedAt)
            .Select(n => AllocationNeedCandidateRanker.Rank(
                n,
                data,
                request.MaxResultsPerNeed,
                filters))
            .ToList();

        return new ProjectMatchingCandidatesDto(
            project.Id,
            project.Name,
            rankedNeeds);
    }
}
