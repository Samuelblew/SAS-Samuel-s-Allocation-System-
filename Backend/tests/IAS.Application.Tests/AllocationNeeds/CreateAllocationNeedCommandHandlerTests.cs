using IAS.Application.AllocationNeeds;
using IAS.Application.AllocationNeeds.Commands.CreateAllocationNeed;
using IAS.Application.Common.Exceptions;
using IAS.Application.Common.Interfaces;
using IAS.Domain.AllocationNeeds;
using NSubstitute;

namespace IAS.Application.Tests.AllocationNeeds;

public sealed class CreateAllocationNeedCommandHandlerTests
{
    [Fact]
    public async Task Handle_ProjetoInexistente_LancaNotFoundException()
    {
        var repository = Substitute.For<IAllocationNeedRepository>();
        repository.ProjectExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(false);

        var tenant = Substitute.For<ITenantContext>();
        tenant.IsResolved.Returns(true);
        tenant.TenantId.Returns(Guid.NewGuid());

        var handler = new CreateAllocationNeedCommandHandler(repository, tenant);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(
                new CreateAllocationNeedCommand(
                    Guid.NewGuid(),
                    "Backend",
                    "Senior",
                    [],
                    [],
                    50m,
                    null,
                    null,
                    AllocationNeedUrgency.High,
                    AllocationNeedCriticality.Medium,
                    AllocationNeedStatus.Open),
                CancellationToken.None));
    }
}
