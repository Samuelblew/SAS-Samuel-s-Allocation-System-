using IAS.Application.Common.Exceptions;
using IAS.Application.Common.Interfaces;
using IAS.Application.Projects;
using IAS.Application.Projects.Commands.CreateProject;
using IAS.Domain.Projects;
using NSubstitute;

namespace IAS.Application.Tests.Projects;

public sealed class CreateProjectCommandHandlerTests
{
    [Fact]
    public async Task Handle_ClienteInexistente_LancaNotFoundException()
    {
        var repository = Substitute.For<IProjectRepository>();
        repository.ClientExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        var tenant = Substitute.For<ITenantContext>();
        tenant.IsResolved.Returns(true);
        tenant.TenantId.Returns(Guid.NewGuid());

        var handler = new CreateProjectCommandHandler(repository, tenant);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(
                new CreateProjectCommand(
                    Guid.NewGuid(),
                    "Projeto X",
                    ProjectStatus.Proposal,
                    null,
                    null,
                    ProjectPriority.Medium,
                    null,
                    null,
                    null,
                    null,
                    null),
                CancellationToken.None));
    }
}
