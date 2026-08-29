using IAS.Domain.People;

namespace IAS.Application.Financial;

public static class PersonCostResolver
{
    public const decimal DefaultMonthlyHours = 160m;

    public static decimal? ResolveHourlyRate(Person person) =>
        person.HourlyCost
        ?? (person.MonthlyCost.HasValue ? person.MonthlyCost.Value / DefaultMonthlyHours : null);
}
