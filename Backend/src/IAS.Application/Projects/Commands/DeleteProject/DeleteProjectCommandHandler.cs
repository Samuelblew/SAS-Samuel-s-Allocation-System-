using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.Projects.Commands.DeleteProject;

public sealed class DeleteProjectCommandHandler(IProjectRepository repository)
    : IRequestHandler<DeleteProjectCommand>
{
    public async Task Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Projeto '{request.Id}' não encontrado.");

        project.DeletedAt = DateTime.UtcNow;
        project.UpdatedAt = project.DeletedAt;

        await repository.SaveChangesAsync(cancellationToken);
    }
}
