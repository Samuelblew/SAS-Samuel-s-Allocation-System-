using IAS.Application.Common.Exceptions;
using IAS.Application.Projects;
using MediatR;

namespace IAS.Application.Projects.Queries.GetProjectById;

public sealed class GetProjectByIdQueryHandler(IProjectRepository repository)
    : IRequestHandler<GetProjectByIdQuery, ProjectDto>
{
    public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Projeto '{request.Id}' não encontrado.");

        return project.ToDto();
    }
}
