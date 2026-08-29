namespace IAS.Application.Capacity;

internal static class CapacityDataLoader
{
    public static async Task<CapacityPeriodData> LoadAsync(
        ICapacityReadRepository repository,
        DateOnly from,
        DateOnly to,
        bool includeSkills,
        CancellationToken cancellationToken)
    {
        var people = includeSkills
            ? await repository.ListActivePeopleWithSkillsAsync(cancellationToken)
            : await repository.ListActivePeopleAsync(cancellationToken);

        var allocations = await repository.GetAllocationsInPeriodAsync(from, to, cancellationToken);
        var unavailabilities = await repository.GetUnavailabilitiesInPeriodAsync(from, to, cancellationToken);

        return new CapacityPeriodData(people, allocations, unavailabilities);
    }
}
