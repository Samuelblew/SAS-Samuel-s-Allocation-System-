using IAS.Application.Allocations;
using IAS.Domain.Allocations;

namespace IAS.Application.Tests.Allocations;

public sealed class AllocationOverloadCheckerTests
{
    [Fact]
    public void WouldExceedWeeklyCapacity_SomaMaiorQue100_RetornaTrue()
    {
        var existing = new List<Allocation>
        {
            CreateAllocation(60, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 30))
        };

        var result = AllocationOverloadChecker.WouldExceedWeeklyCapacity(
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 30),
            50,
            existing);

        Assert.True(result);
    }

    [Fact]
    public void WouldExceedWeeklyCapacity_AlocacaoEncerrada_NaoConta()
    {
        var existing = new List<Allocation>
        {
            CreateAllocation(80, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 30), AllocationStatus.Closed)
        };

        var result = AllocationOverloadChecker.WouldExceedWeeklyCapacity(
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 30),
            50,
            existing);

        Assert.False(result);
    }

    [Fact]
    public void WouldExceedWeeklyCapacity_SemAlocacoes_50PorCento_RetornaFalse()
    {
        var result = AllocationOverloadChecker.WouldExceedWeeklyCapacity(
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 30),
            50,
            []);

        Assert.False(result);
    }

    [Fact]
    public void FindFirstOverloadWeek_RetornaDetalhesDaSemana()
    {
        var existing = new List<Allocation>
        {
            CreateAllocation(91, new DateOnly(2026, 6, 1), new DateOnly(2026, 6, 30))
        };

        var overload = AllocationOverloadChecker.FindFirstOverloadWeek(
            new DateOnly(2026, 6, 1),
            new DateOnly(2026, 6, 30),
            50,
            existing);

        Assert.NotNull(overload);
        Assert.Equal(91, overload.ExistingPercent);
        Assert.Equal(141, overload.TotalPercent);
    }

    private static Allocation CreateAllocation(
        decimal dedication,
        DateOnly start,
        DateOnly end,
        AllocationStatus status = AllocationStatus.Confirmed) =>
        new()
        {
            Id = Guid.NewGuid(),
            DedicationPercent = dedication,
            StartDate = start,
            EndDate = end,
            Status = status
        };
}
