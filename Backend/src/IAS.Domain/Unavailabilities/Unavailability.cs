using IAS.Domain.Common;
using IAS.Domain.People;

namespace IAS.Domain.Unavailabilities;

public class Unavailability : TenantEntity
{
    public Guid PersonId { get; set; }
    public Person Person { get; set; } = null!;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public UnavailabilityType Type { get; set; }
    public string? Notes { get; set; }
}
