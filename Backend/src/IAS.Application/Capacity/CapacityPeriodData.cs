using IAS.Domain.Allocations;
using IAS.Domain.People;
using IAS.Domain.Unavailabilities;

namespace IAS.Application.Capacity;

public sealed record CapacityPeriodData(
    IReadOnlyList<Person> People,
    IReadOnlyList<Allocation> Allocations,
    IReadOnlyList<Unavailability> Unavailabilities);
