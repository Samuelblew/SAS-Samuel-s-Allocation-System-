using IAS.Domain.Common;
using IAS.Domain.People;
using IAS.Domain.Projects;

namespace IAS.Domain.Allocations;

public class Allocation : TenantEntity
{
    public Guid PersonId { get; set; }
    public Person Person { get; set; } = null!;
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string Role { get; set; } = string.Empty;
    public decimal DedicationPercent { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public AllocationStatus Status { get; set; } = AllocationStatus.Planned;
    public string? Notes { get; set; }
}
