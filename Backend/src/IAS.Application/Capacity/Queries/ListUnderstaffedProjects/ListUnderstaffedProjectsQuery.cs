using IAS.Domain.Projects;
using MediatR;

namespace IAS.Application.Capacity.Queries.ListUnderstaffedProjects;

public sealed record ListUnderstaffedProjectsQuery() : IRequest<UnderstaffedProjectsListDto>;

public sealed record UnderstaffedProjectsListDto(IReadOnlyList<UnderstaffedProjectDto> Items);

public sealed record UnderstaffedProjectDto(
    Guid ProjectId,
    string ProjectName,
    ProjectStatus Status,
    int OpenNeedsCount,
    decimal TotalGapPercent);
