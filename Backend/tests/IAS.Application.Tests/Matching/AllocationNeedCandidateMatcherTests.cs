using IAS.Application.Capacity;
using IAS.Application.Matching;
using IAS.Domain.AllocationNeeds;
using IAS.Domain.Allocations;
using IAS.Domain.People;
using IAS.Domain.Projects;

namespace IAS.Application.Tests.Matching;

public sealed class AllocationNeedCandidateMatcherTests
{
    private static readonly Guid SkillJava = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid ProjectId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    [Fact]
    public void Rank_OrdersBySkillsAndAvailability()
    {
        var need = CreateNeed(["Java skill"], []);
        var strong = CreatePerson("Strong", "Senior", [SkillJava], hourlyCost: 80);
        var weak = CreatePerson("Weak", "Junior", [], hourlyCost: 120);

        var data = new CapacityPeriodData([strong, weak], [], []);
        var ranked = AllocationNeedCandidateMatcher.Rank(need, "Consulting", data);

        Assert.Equal(2, ranked.Count);
        Assert.Equal(strong.Id, ranked[0].PersonId);
        Assert.True(ranked[0].Breakdown.TotalScore > ranked[1].Breakdown.TotalScore);
        Assert.True(ranked[0].Breakdown.RequiredSkillsScore > ranked[1].Breakdown.RequiredSkillsScore);
    }

    [Fact]
    public void Rank_FiltersByMinAvailablePercent()
    {
        var need = CreateNeed([], [], dedication: 50);
        var available = CreatePerson("Free", "Senior", [], hourlyCost: 100);
        var busy = CreatePerson("Busy", "Senior", [], hourlyCost: 100);
        var busyAllocation = new Allocation
        {
            Id = Guid.NewGuid(),
            TenantId = busy.TenantId,
            PersonId = busy.Id,
            ProjectId = Guid.NewGuid(),
            Role = "Backend",
            DedicationPercent = 90m,
            StartDate = new DateOnly(2026, 6, 1),
            EndDate = new DateOnly(2026, 12, 31),
            Status = AllocationStatus.Confirmed
        };

        var data = new CapacityPeriodData([available, busy], [busyAllocation], []);
        var filters = new CandidateMatchFilters(MinAvailablePercent: 50m);
        var ranked = AllocationNeedCandidateMatcher.Rank(need, "Consulting", data, filters: filters);

        Assert.Single(ranked);
        Assert.Equal(available.Id, ranked[0].PersonId);
    }

    [Fact]
    public void Rank_FlagsPeopleAlreadyOnProject()
    {
        var need = CreateNeed([], []);
        var insider = CreatePerson("Insider", "Senior", [], hourlyCost: 100);
        var outsider = CreatePerson("Outsider", "Senior", [], hourlyCost: 100);
        var projectAllocation = new Allocation
        {
            Id = Guid.NewGuid(),
            TenantId = insider.TenantId,
            PersonId = insider.Id,
            ProjectId = ProjectId,
            Role = "Backend",
            DedicationPercent = 50m,
            StartDate = new DateOnly(2026, 6, 1),
            EndDate = new DateOnly(2026, 12, 31),
            Status = AllocationStatus.Confirmed
        };

        var data = new CapacityPeriodData([insider, outsider], [projectAllocation], []);
        var ranked = AllocationNeedCandidateMatcher.Rank(need, "Consulting", data);

        Assert.Equal(2, ranked.Count);
        var flagged = ranked.Single(c => c.PersonId == insider.Id);
        Assert.True(flagged.AlreadyOnProject);
        Assert.Equal(50m, flagged.ProjectDedicationPercent);
        Assert.False(ranked.Single(c => c.PersonId == outsider.Id).AlreadyOnProject);
    }

    [Fact]
    public void Rank_ExcludesPeopleAlreadyOnProject()
    {
        var need = CreateNeed([], []);
        var insider = CreatePerson("Insider", "Senior", [], hourlyCost: 100);
        var outsider = CreatePerson("Outsider", "Senior", [], hourlyCost: 100);
        var projectAllocation = new Allocation
        {
            Id = Guid.NewGuid(),
            TenantId = insider.TenantId,
            PersonId = insider.Id,
            ProjectId = ProjectId,
            Role = "Backend",
            DedicationPercent = 50m,
            StartDate = new DateOnly(2026, 6, 1),
            EndDate = new DateOnly(2026, 12, 31),
            Status = AllocationStatus.Confirmed
        };

        var data = new CapacityPeriodData([insider, outsider], [projectAllocation], []);
        var filters = new CandidateMatchFilters(ExcludePeopleOnProject: true);
        var ranked = AllocationNeedCandidateMatcher.Rank(need, "Consulting", data, filters: filters);

        Assert.Single(ranked);
        Assert.Equal(outsider.Id, ranked[0].PersonId);
    }

    [Fact]
    public void Rank_AppliesOverloadPenalty()
    {
        var need = CreateNeed([], [], dedication: 50);
        var person = CreatePerson("Busy", "Senior", [], hourlyCost: 100);
        var allocation = new Allocation
        {
            Id = Guid.NewGuid(),
            TenantId = person.TenantId,
            PersonId = person.Id,
            ProjectId = ProjectId,
            Role = "Backend",
            DedicationPercent = 80,
            StartDate = new DateOnly(2026, 6, 1),
            EndDate = new DateOnly(2026, 12, 31),
            Status = AllocationStatus.Confirmed,
            CreatedAt = DateTime.UtcNow
        };

        var data = new CapacityPeriodData([person], [allocation], []);
        var ranked = AllocationNeedCandidateMatcher.Rank(need, null, data);

        Assert.Single(ranked);
        Assert.True(ranked[0].Breakdown.OverloadPenalty > 0);
    }

    private static AllocationNeed CreateNeed(
        string[] requiredSkillNames,
        string[] desiredSkillNames,
        decimal dedication = 50)
    {
        var required = requiredSkillNames.Length == 0 ? [] : new List<Guid> { SkillJava };
        return new AllocationNeed
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            ProjectId = ProjectId,
            Project = new Project { Id = ProjectId, Name = "Proj", ProjectType = "Consulting" },
            Role = "Backend",
            ExpectedSeniority = "Senior",
            RequiredSkillIds = required,
            DesiredSkillIds = [],
            DedicationPercent = dedication,
            StartDate = new DateOnly(2026, 6, 1),
            EndDate = new DateOnly(2026, 12, 31),
            CreatedAt = DateTime.UtcNow
        };
    }

    private static Person CreatePerson(
        string name,
        string seniority,
        Guid[] skillIds,
        decimal hourlyCost)
    {
        var person = new Person
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            Name = name,
            Seniority = seniority,
            HourlyCost = hourlyCost,
            Status = PersonStatus.Active,
            WeeklyCapacityHours = 40,
            CreatedAt = DateTime.UtcNow,
            Skills = skillIds.Select(id => new PersonSkill
            {
                Id = Guid.NewGuid(),
                TenantId = Guid.NewGuid(),
                SkillId = id,
                Level = SkillProficiencyLevel.Advanced,
                CreatedAt = DateTime.UtcNow
            }).ToList()
        };

        return person;
    }
}
