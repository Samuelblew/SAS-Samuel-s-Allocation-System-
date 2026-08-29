using IAS.Domain.Common;
using IAS.Domain.Projects;

namespace IAS.Domain.Clients;

public class Client : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Notes { get; set; }

    public ICollection<Project> Projects { get; set; } = [];
}
