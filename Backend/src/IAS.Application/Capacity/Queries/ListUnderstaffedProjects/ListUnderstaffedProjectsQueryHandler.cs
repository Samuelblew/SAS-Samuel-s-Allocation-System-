using MediatR;

namespace IAS.Application.Capacity.Queries.ListUnderstaffedProjects;

public sealed class ListUnderstaffedProjectsQueryHandler(ICapacityReadRepository repository)
    : IRequestHandler<ListUnderstaffedProjectsQuery, UnderstaffedProjectsListDto>
{
    public async Task<UnderstaffedProjectsListDto> Handle(
        ListUnderstaffedProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var summaries = await repository.ListProjectStaffingSummariesAsync(cancellationToken);

        return new UnderstaffedProjectsListDto(
            summaries.Select(s => new UnderstaffedProjectDto(
                s.ProjectId,
                s.ProjectName,
                s.Status,
                s.OpenNeedsCount,
                s.TotalGapPercent)).ToList());
    }
}
