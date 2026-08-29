using IAS.Application.Projects;
using MediatR;

namespace IAS.Application.Projects.Queries.GetProjectById;

public sealed record GetProjectByIdQuery(Guid Id) : IRequest<ProjectDto>;
