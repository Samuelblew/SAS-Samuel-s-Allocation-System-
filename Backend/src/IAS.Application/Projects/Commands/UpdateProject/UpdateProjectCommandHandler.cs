using IAS.Application.Common.Exceptions;
using IAS.Application.Projects;
using MediatR;

namespace IAS.Application.Projects.Commands.UpdateProject;

public sealed class UpdateProjectCommandHandler(IProjectRepository repository)
    : IRequestHandler<UpdateProjectCommand, ProjectDto>
{
    public async Task<ProjectDto> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Projeto '{request.Id}' não encontrado.");

        if (!await repository.ClientExistsAsync(request.ClientId, cancellationToken))
            throw new NotFoundException($"Cliente '{request.ClientId}' não encontrado.");

        project.ClientId = request.ClientId;
        project.Name = request.Name.Trim();
        project.Status = request.Status;
        project.StartDate = request.StartDate;
        project.EndDate = request.EndDate;
        project.Priority = request.Priority;
        project.Budget = request.Budget;
        project.EstimatedRevenue = request.EstimatedRevenue;
        project.ProjectType = TrimOrNull(request.ProjectType);
        project.CommercialOwner = TrimOrNull(request.CommercialOwner);
        project.DeliveryOwner = TrimOrNull(request.DeliveryOwner);
        project.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync(cancellationToken);

        var loaded = await repository.GetByIdAsync(project.Id, cancellationToken)
            ?? throw new InvalidOperationException("Falha ao carregar projeto atualizado.");

        return loaded.ToDto();
    }

    private static string? TrimOrNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
