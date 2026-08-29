using IAS.Application.Capacity;

namespace IAS.Application.Tests.Capacity;

public sealed class EffectiveCapacityCalculatorTests
{
    [Fact]
    public void FromWeek_ConvertsPercentToHours()
    {
        var week = new WeekAvailability(
            new DateOnly(2026, 6, 2),
            new DateOnly(2026, 6, 8),
            AllocatedPercent: 50m,
            AvailablePercent: 50m,
            IsUnavailable: false);

        var result = EffectiveCapacityCalculator.FromWeek(week, weeklyCapacityHours: 40m);

        Assert.Equal(40m, result.WeeklyCapacityHours);
        Assert.Equal(20m, result.AllocatedHours);
        Assert.Equal(20m, result.AvailableHours);
        Assert.False(result.IsUnavailable);
    }

    [Fact]
    public void FromWeek_ReturnsZeroAvailableWhenUnavailable()
    {
        var week = new WeekAvailability(
            new DateOnly(2026, 6, 2),
            new DateOnly(2026, 6, 8),
            AllocatedPercent: 0m,
            AvailablePercent: 0m,
            IsUnavailable: true);

        var result = EffectiveCapacityCalculator.FromWeek(week, weeklyCapacityHours: 40m);

        Assert.Equal(0m, result.AvailableHours);
    }
}
