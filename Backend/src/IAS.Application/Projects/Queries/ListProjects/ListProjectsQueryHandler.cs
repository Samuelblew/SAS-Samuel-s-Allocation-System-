using IAS.Application.Common.Models;
using IAS.Application.Projects;
using MediatR;

namespace IAS.Application.Projects.Queries.ListProjects;

public sealed class ListProjectsQueryHandler(IProjectRepository repository)
    : IRequestHandler<ListProjectsQuery, PagedResult<ProjectListItemDto>>
{
    public async Task<PagedResult<ProjectListItemDto>> Handle(
        ListProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var (items, total) = await repository.ListAsync(
            request.Page,
            request.PageSize,
            request.ClientId,
            cancellationToken);

        return new PagedResult<ProjectListItemDto>(
            items.Select(p => p.ToListItemDto()).ToList(),
            request.Page,
            request.PageSize,
            total);
    }
}
