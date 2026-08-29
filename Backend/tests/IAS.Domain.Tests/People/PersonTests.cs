using IAS.Domain.People;

namespace IAS.Domain.Tests.People;

public sealed class PersonTests
{
    [Fact]
    public void Person_StatusPadraoEhActive()
    {
        var person = new Person { Name = "Ana" };
        Assert.Equal(PersonStatus.Active, person.Status);
        Assert.Equal(40, person.WeeklyCapacityHours);
    }
}
