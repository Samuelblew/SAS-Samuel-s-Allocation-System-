using IAS.Application.AllocationNeeds;
using IAS.Application.Allocations;
using IAS.Application.Allocations.Commands.CreateAllocation;
using IAS.Application.Common.Exceptions;
using IAS.Application.Common.Interfaces;
using IAS.Domain.Allocations;
using IAS.Domain.People;
using NSubstitute;

namespace IAS.Application.Tests.Allocations;

public sealed class CreateAllocationCommandHandlerTests
{
    [Fact]
    public async Task Handle_Superalocacao_LancaConflictException()
    {
        var personId = Guid.NewGuid();
        var existing = new Allocation
        {
            Id = Guid.NewGuid(),
            PersonId = personId,
            DedicationPercent = 70,
            StartDate = new DateOnly(2026, 6, 1),
            EndDate = new DateOnly(2026, 6, 30),
            Status = AllocationStatus.Confirmed
        };

        var repository = Substitute.For<IAllocationRepository>();
        repository.GetPersonAsync(personId, Arg.Any<CancellationToken>())
            .Returns(new Person { Id = personId, Status = PersonStatus.Active });
        repository.ProjectExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        repository.GetOverlappingForPersonAsync(
                personId,
                Arg.Any<DateOnly>(),
                Arg.Any<DateOnly>(),
                null,
                Arg.Any<CancellationToken>())
            .Returns([existing]);

        var tenant = Substitute.For<ITenantContext>();
        tenant.IsResolved.Returns(true);
        tenant.TenantId.Returns(Guid.NewGuid());

        var needSync = Substitute.For<IAllocationNeedStatusSync>();
        var handler = new CreateAllocationCommandHandler(repository, needSync, tenant);

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(
                new CreateAllocationCommand(
                    personId,
                    Guid.NewGuid(),
                    "Backend",
                    50,
                    new DateOnly(2026, 6, 1),
                    new DateOnly(2026, 6, 30),
                    AllocationStatus.Planned,
                    null),
                CancellationToken.None));
    }

    [Fact]
    public async Task Handle_PessoaOffboarded_LancaConflictException()
    {
        var personId = Guid.NewGuid();
        var repository = Substitute.For<IAllocationRepository>();
        repository.GetPersonAsync(personId, Arg.Any<CancellationToken>())
            .Returns(new Person { Id = personId, Status = PersonStatus.Offboarded });

        var tenant = Substitute.For<ITenantContext>();
        tenant.IsResolved.Returns(true);

        var needSync = Substitute.For<IAllocationNeedStatusSync>();
        var handler = new CreateAllocationCommandHandler(repository, needSync, tenant);

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(
                new CreateAllocationCommand(
                    personId,
                    Guid.NewGuid(),
                    "Backend",
                    50,
                    new DateOnly(2026, 6, 1),
                    new DateOnly(2026, 6, 30),
                    AllocationStatus.Planned,
                    null),
                CancellationToken.None));
    }
}
