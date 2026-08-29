using MediatR;

namespace IAS.Application.Projects.Commands.DeleteProject;

public sealed record DeleteProjectCommand(Guid Id) : IRequest;
