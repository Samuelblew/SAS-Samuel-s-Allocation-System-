using IAS.Application.Capacity;
using IAS.Domain.Allocations;
using IAS.Domain.People;
using IAS.Domain.Skills;

namespace IAS.Application.Tests.Capacity;

public sealed class SkillOccupationCalculatorTests
{
    private static readonly Guid SkillJava = Guid.Parse("11111111-1111-1111-1111-111111111111");

    [Fact]
    public void Calculate_AggregatesBySkill()
    {
        var javaDev = CreatePerson("Java Dev", SkillJava, "Java");
        var otherDev = CreatePerson("Other Dev", Guid.NewGuid(), "Python");

        var allocation = new Allocation
        {
            Id = Guid.NewGuid(),
            TenantId = javaDev.TenantId,
            PersonId = javaDev.Id,
            ProjectId = Guid.NewGuid(),
            Role = "Backend",
            DedicationPercent = 80m,
            StartDate = new DateOnly(2026, 6, 1),
            EndDate = new DateOnly(2026, 6, 30),
            Status = AllocationStatus.Confirmed,
            CreatedAt = DateTime.UtcNow
        };

        var data = new CapacityPeriodData([javaDev, otherDev], [allocation], []);
        var result = SkillOccupationCalculator.Calculate(
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 30),
            data);

        Assert.Equal(2, result.Count);
        var java = result.Single(s => s.SkillId == SkillJava);
        Assert.Equal(1, java.PeopleCount);
        Assert.True(java.AvgAllocatedPercent > 0);
        Assert.True(java.AvgAllocatedHours > 0);
    }

    private static Person CreatePerson(string name, Guid skillId, string skillName)
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            Name = name,
            WeeklyCapacityHours = 40,
            Status = PersonStatus.Active,
            CreatedAt = DateTime.UtcNow,
            Skills =
            [
                new PersonSkill
                {
                    Id = Guid.NewGuid(),
                    TenantId = Guid.NewGuid(),
                    SkillId = skillId,
                    Skill = new Skill { Id = skillId, Name = skillName, TenantId = Guid.NewGuid(), CreatedAt = DateTime.UtcNow },
                    Level = SkillProficiencyLevel.Advanced,
                    CreatedAt = DateTime.UtcNow
                }
            ]
        };

        return person;
    }
}
