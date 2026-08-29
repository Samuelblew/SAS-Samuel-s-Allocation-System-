using IAS.Domain.Common;

namespace IAS.Domain.Skills;

public class Skill : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Category { get; set; }
}
