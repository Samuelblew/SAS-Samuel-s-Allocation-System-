using IAS.Application.Allocations;
using IAS.Domain.Allocations;
using IAS.Domain.People;
using IAS.Domain.Projects;

namespace IAS.Application.Tests.Allocations;

public sealed class AllocationConflictDetectorTests
{
    [Fact]
    public void DetectWeeklyConflicts_Superalocacao_RetornaConflito()
    {
        var personId = Guid.NewGuid();
        var person = new Person { Id = personId, Name = "Ana" };
        var projectA = new Project { Id = Guid.NewGuid(), Name = "Projeto A" };
        var projectB = new Project { Id = Guid.NewGuid(), Name = "Projeto B" };

        var allocations = new List<Allocation>
        {
            CreateAllocation(person, projectA, 60, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 30)),
            CreateAllocation(person, projectB, 50, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 30))
        };

        var conflicts = AllocationOverloadChecker.DetectWeeklyConflicts(allocations);

        Assert.NotEmpty(conflicts);
        Assert.Equal(110, conflicts[0].TotalDedicationPercent);
        Assert.Equal(2, conflicts[0].Allocations.Count);
    }

    private static Allocation CreateAllocation(
        Person person,
        Project project,
        decimal dedication,
        DateOnly start,
        DateOnly end) =>
        new()
        {
            Id = Guid.NewGuid(),
            PersonId = person.Id,
            Person = person,
            ProjectId = project.Id,
            Project = project,
            DedicationPercent = dedication,
            StartDate = start,
            EndDate = end,
            Status = AllocationStatus.Confirmed
        };
}
