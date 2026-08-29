using IAS.Domain.Common;

namespace IAS.Domain.People;

public class Person : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public string? Seniority { get; set; }
    public decimal? HourlyCost { get; set; }
    public decimal? MonthlyCost { get; set; }
    public decimal WeeklyCapacityHours { get; set; } = 40;
    public string? Location { get; set; }
    public string? Team { get; set; }
    public PersonStatus Status { get; set; } = PersonStatus.Active;

    public ICollection<PersonSkill> Skills { get; set; } = [];
}
