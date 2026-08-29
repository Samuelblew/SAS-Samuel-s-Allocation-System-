using IAS.Domain.Common;
using IAS.Domain.Projects;

namespace IAS.Domain.AllocationNeeds;

public class AllocationNeed : TenantEntity
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
    public string Role { get; set; } = string.Empty;
    public string? ExpectedSeniority { get; set; }
    public List<Guid> RequiredSkillIds { get; set; } = [];
    public List<Guid> DesiredSkillIds { get; set; } = [];
    public decimal DedicationPercent { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public AllocationNeedUrgency Urgency { get; set; } = AllocationNeedUrgency.Medium;
    public AllocationNeedCriticality Criticality { get; set; } = AllocationNeedCriticality.Medium;
    public AllocationNeedStatus Status { get; set; } = AllocationNeedStatus.Open;
}
