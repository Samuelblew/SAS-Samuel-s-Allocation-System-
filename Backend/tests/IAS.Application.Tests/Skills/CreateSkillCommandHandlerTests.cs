using IAS.Application.Common.Exceptions;
using IAS.Application.Common.Interfaces;
using IAS.Application.Skills;
using IAS.Application.Skills.Commands.CreateSkill;
using IAS.Domain.Skills;
using NSubstitute;

namespace IAS.Application.Tests.Skills;

public sealed class CreateSkillCommandHandlerTests
{
    private static readonly Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public async Task Handle_QuandoNomeDuplicado_LancaConflictException()
    {
        var repository = Substitute.For<ISkillRepository>();
        repository.ExistsByNameAsync("C#", null, Arg.Any<CancellationToken>()).Returns(true);

        var tenant = Substitute.For<ITenantContext>();
        tenant.IsResolved.Returns(true);
        tenant.TenantId.Returns(TenantId);

        var handler = new CreateSkillCommandHandler(repository, tenant);

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(new CreateSkillCommand("C#", "Backend"), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_QuandoValido_PersisteSkillComTenant()
    {
        var repository = Substitute.For<ISkillRepository>();
        repository.ExistsByNameAsync(Arg.Any<string>(), null, Arg.Any<CancellationToken>()).Returns(false);

        Skill? captured = null;
        await repository.AddAsync(Arg.Do<Skill>(s => captured = s), Arg.Any<CancellationToken>());

        var tenant = Substitute.For<ITenantContext>();
        tenant.IsResolved.Returns(true);
        tenant.TenantId.Returns(TenantId);

        var handler = new CreateSkillCommandHandler(repository, tenant);

        var result = await handler.Handle(new CreateSkillCommand("  React  ", " Frontend "), CancellationToken.None);

        Assert.NotNull(captured);
        Assert.Equal(TenantId, captured.TenantId);
        Assert.Equal("React", captured.Name);
        Assert.Equal("Frontend", captured.Category);
        Assert.Equal("React", result.Name);
        await repository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
