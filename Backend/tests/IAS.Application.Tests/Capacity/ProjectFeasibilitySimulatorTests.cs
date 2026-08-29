using IAS.Application.Capacity;
using IAS.Domain.Allocations;
using IAS.Domain.People;

namespace IAS.Application.Tests.Capacity;

public sealed class ProjectFeasibilitySimulatorTests
{
    [Fact]
    public void Simulate_Feasible_WhenEnoughAvailablePeople()
    {
        var people = new List<Person>
        {
            CreatePerson("Alice", "Senior"),
            CreatePerson("Bob", "Senior")
        };

        var data = new CapacityPeriodData(people, [], []);
        var needs = new[] { new SimulatedNeed("Backend", "Senior", [], 50, 2) };

        var result = ProjectFeasibilitySimulator.Simulate(
            new DateOnly(2026, 7, 1), 3, needs, data);

        Assert.True(result.FeasibleAtDesiredStart);
        Assert.Equal(new DateOnly(2026, 7, 1), result.EarliestFeasibleStart);
    }

    [Fact]
    public void Simulate_NotFeasible_WhenInsufficientCandidates()
    {
        var people = new List<Person> { CreatePerson("Alice", "Senior") };
        var data = new CapacityPeriodData(people, [], []);
        var needs = new[] { new SimulatedNeed("Backend", "Senior", [], 50, 2) };

        var result = ProjectFeasibilitySimulator.Simulate(
            new DateOnly(2026, 7, 1), 3, needs, data);

        Assert.False(result.FeasibleAtDesiredStart);
        Assert.Null(result.EarliestFeasibleStart);
    }

    [Fact]
    public void Simulate_NotFeasible_WhenPersonHasNoSeniority()
    {
        var people = new List<Person> { CreatePerson("Alice", null) };
        var data = new CapacityPeriodData(people, [], []);
        var needs = new[] { new SimulatedNeed("Backend", "Senior", [], 50, 1) };

        var result = ProjectFeasibilitySimulator.Simulate(
            new DateOnly(2026, 7, 1), 3, needs, data);

        Assert.False(result.FeasibleAtDesiredStart);
    }

    private static Person CreatePerson(string name, string? seniority) =>
        new()
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            Name = name,
            Seniority = seniority,
            Status = PersonStatus.Active,
            WeeklyCapacityHours = 40,
            CreatedAt = DateTime.UtcNow,
            Skills = []
        };
}
