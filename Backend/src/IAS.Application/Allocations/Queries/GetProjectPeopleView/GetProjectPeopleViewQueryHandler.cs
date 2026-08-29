using IAS.Application.Allocations;
using IAS.Application.Common.Exceptions;
using IAS.Application.Projects;
using MediatR;

namespace IAS.Application.Allocations.Queries.GetProjectPeopleView;

public sealed class GetProjectPeopleViewQueryHandler(
    IAllocationRepository allocationRepository,
    IProjectRepository projectRepository) : IRequestHandler<GetProjectPeopleViewQuery, ProjectPeopleViewDto>
{
    public async Task<ProjectPeopleViewDto> Handle(
        GetProjectPeopleViewQuery request,
        CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetByIdAsync(request.ProjectId, cancellationToken)
            ?? throw new NotFoundException($"Projeto '{request.ProjectId}' não encontrado.");

        var allocations = await allocationRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);
        return AllocationViewMapping.ToProjectPeopleView(allocations, project.Id, project.Name);
    }
}
