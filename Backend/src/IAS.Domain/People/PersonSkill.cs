using IAS.Domain.Common;
using IAS.Domain.Skills;

namespace IAS.Domain.People;

public class PersonSkill : TenantEntity
{
    public Guid PersonId { get; set; }
    public Person Person { get; set; } = null!;
    public Guid SkillId { get; set; }
    public Skill Skill { get; set; } = null!;
    public SkillProficiencyLevel Level { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public string? Notes { get; set; }
}
