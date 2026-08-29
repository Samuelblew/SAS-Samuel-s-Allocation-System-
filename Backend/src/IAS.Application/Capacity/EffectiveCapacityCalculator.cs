namespace IAS.Application.Capacity;

public sealed record WeekCapacityHours(
    DateOnly WeekStart,
    DateOnly WeekEnd,
    decimal WeeklyCapacityHours,
    decimal AllocatedHours,
    decimal AvailableHours,
    bool IsUnavailable);

public static class EffectiveCapacityCalculator
{
    public static WeekCapacityHours FromWeek(WeekAvailability week, decimal weeklyCapacityHours) =>
        new(
            week.WeekStart,
            week.WeekEnd,
            weeklyCapacityHours,
            Math.Round(weeklyCapacityHours * week.AllocatedPercent / 100m, 2),
            week.IsUnavailable
                ? 0
                : Math.Round(weeklyCapacityHours * week.AvailablePercent / 100m, 2),
            week.IsUnavailable);
}
