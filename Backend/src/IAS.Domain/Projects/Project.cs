using IAS.Domain.Clients;
using IAS.Domain.Common;

namespace IAS.Domain.Projects;

public class Project : TenantEntity
{
    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public ProjectStatus Status { get; set; } = ProjectStatus.Proposal;
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public ProjectPriority Priority { get; set; } = ProjectPriority.Medium;
    public decimal? Budget { get; set; }
    public decimal? EstimatedRevenue { get; set; }
    public string? ProjectType { get; set; }
    public string? CommercialOwner { get; set; }
    public string? DeliveryOwner { get; set; }
}
