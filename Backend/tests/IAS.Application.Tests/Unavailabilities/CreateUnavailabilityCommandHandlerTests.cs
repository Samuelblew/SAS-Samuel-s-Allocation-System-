using IAS.Application.Common.Exceptions;
using IAS.Application.Common.Interfaces;
using IAS.Application.Unavailabilities;
using IAS.Application.Unavailabilities.Commands.CreateUnavailability;
using IAS.Domain.Unavailabilities;
using NSubstitute;

namespace IAS.Application.Tests.Unavailabilities;

public sealed class CreateUnavailabilityCommandHandlerTests
{
    private static readonly Guid TenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public async Task Handle_PeriodoSobreposto_LancaConflictException()
    {
        var repository = Substitute.For<IUnavailabilityRepository>();
        repository.PersonExistsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(true);
        repository.HasOverlapAsync(Arg.Any<Guid>(), Arg.Any<DateOnly>(), Arg.Any<DateOnly>(), null, Arg.Any<CancellationToken>())
            .Returns(true);

        var tenant = Substitute.For<ITenantContext>();
        tenant.IsResolved.Returns(true);
        tenant.TenantId.Returns(TenantId);

        var handler = new CreateUnavailabilityCommandHandler(repository, tenant);

        await Assert.ThrowsAsync<ConflictException>(() =>
            handler.Handle(
                new CreateUnavailabilityCommand(
                    Guid.NewGuid(),
                    new DateOnly(2026, 7, 1),
                    new DateOnly(2026, 7, 15),
                    UnavailabilityType.Vacation,
                    null),
                CancellationToken.None));
    }
}
