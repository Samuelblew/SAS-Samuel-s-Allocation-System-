using IAS.Application.Common.Exceptions;
using IAS.Application.Common.Interfaces;
using IAS.Application.Projects;
using IAS.Domain.Projects;
using MediatR;

namespace IAS.Application.Projects.Commands.CreateProject;

public sealed class CreateProjectCommandHandler(
    IProjectRepository repository,
    ITenantContext tenantContext) : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        if (!tenantContext.IsResolved)
            throw new InvalidOperationException("Tenant não resolvido.");

        if (!await repository.ClientExistsAsync(request.ClientId, cancellationToken))
            throw new NotFoundException($"Cliente '{request.ClientId}' não encontrado.");

        var now = DateTime.UtcNow;
        var project = new Project
        {
            Id = Guid.NewGuid(),
            TenantId = tenantContext.TenantId,
            ClientId = request.ClientId,
            Name = request.Name.Trim(),
            Status = request.Status,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Priority = request.Priority,
            Budget = request.Budget,
            EstimatedRevenue = request.EstimatedRevenue,
            ProjectType = TrimOrNull(request.ProjectType),
            CommercialOwner = TrimOrNull(request.CommercialOwner),
            DeliveryOwner = TrimOrNull(request.DeliveryOwner),
            CreatedAt = now
        };

        await repository.AddAsync(project, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        var loaded = await repository.GetByIdAsync(project.Id, cancellationToken)
            ?? throw new InvalidOperationException("Falha ao carregar projeto criado.");

        return loaded.ToDto();
    }

    private static string? TrimOrNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
