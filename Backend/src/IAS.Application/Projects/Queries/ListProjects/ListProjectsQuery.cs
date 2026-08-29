using IAS.Application.Common.Models;
using IAS.Application.Projects;
using MediatR;

namespace IAS.Application.Projects.Queries.ListProjects;

public sealed record ListProjectsQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? ClientId = null) : IRequest<PagedResult<ProjectListItemDto>>;
