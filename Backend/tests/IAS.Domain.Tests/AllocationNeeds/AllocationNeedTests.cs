using IAS.Domain.AllocationNeeds;

namespace IAS.Domain.Tests.AllocationNeeds;

public sealed class AllocationNeedTests
{
    [Fact]
    public void Novo_RegistraValoresPadrao()
    {
        var need = new AllocationNeed();

        Assert.Equal(AllocationNeedStatus.Open, need.Status);
        Assert.Equal(AllocationNeedUrgency.Medium, need.Urgency);
        Assert.Equal(AllocationNeedCriticality.Medium, need.Criticality);
        Assert.Empty(need.RequiredSkillIds);
        Assert.Empty(need.DesiredSkillIds);
    }
}
