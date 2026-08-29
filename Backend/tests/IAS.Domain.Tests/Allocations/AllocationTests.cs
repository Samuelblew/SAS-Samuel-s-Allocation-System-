using IAS.Domain.Allocations;

namespace IAS.Domain.Tests.Allocations;

public sealed class AllocationTests
{
    [Fact]
    public void Novo_RegistraStatusPlanejada()
    {
        var allocation = new Allocation();

        Assert.Equal(AllocationStatus.Planned, allocation.Status);
    }
}
