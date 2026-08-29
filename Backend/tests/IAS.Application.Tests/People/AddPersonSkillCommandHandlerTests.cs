using IAS.Application.Common.Exceptions;
using IAS.Application.Common.Interfaces;
using IAS.Application.People;
using IAS.Application.People.Commands.AddPersonSkill;
using IAS.Domain.People;
using IAS.Domain.Skills;
using NSubstitute;

namespace IAS.Application.Tests.People;

public sealed class AddPersonSkillCommandHandlerTests
{
    private static readonly Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public async Task Handle_SkillDuplicada_LancaConflictException()
    {
        var repository = Substitute.For<IPersonRepository>();
        repository.GetByIdAsync(Arg.Any<Guid>(), false, Arg.Any<CancellationToken>())
            .Returns(new Person { Id = Guid.NewGuid(), Name = "Ana" });
        repository.SkillCatalogExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        repository.SkillExistsForPersonAsync(Arg.Any<Guid>(), Arg.Any<Guid>(), null, Arg.Any<CancellationToken>())
            .Returns(true);

        var tenant = Substitute.For<ITenantContext>();
        tenant.IsResolved.Returns(true);
        tenant.TenantId.Returns(TenantId);

        var handler = new AddPersonSkillCommandHandler(repository, tenant);

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(
                new AddPersonSkillCommand(Guid.NewGuid(), Guid.NewGuid(), SkillProficiencyLevel.Advanced, null, null),
                CancellationToken.None));
    }
}
