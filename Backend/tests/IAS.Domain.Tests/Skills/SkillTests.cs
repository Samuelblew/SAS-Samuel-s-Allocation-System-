using IAS.Domain.Skills;

namespace IAS.Domain.Tests.Skills;

public sealed class SkillTests
{
    [Fact]
    public void Skill_DeveHerdarCamposDeTenantEntity()
    {
        var id = Guid.NewGuid();
        var tenantId = Guid.NewGuid();
        var created = DateTime.UtcNow;

        var skill = new Skill
        {
            Id = id,
            TenantId = tenantId,
            Name = ".NET",
            Category = "Backend",
            CreatedAt = created
        };

        Assert.Equal(id, skill.Id);
        Assert.Equal(tenantId, skill.TenantId);
        Assert.Equal(".NET", skill.Name);
        Assert.Equal("Backend", skill.Category);
        Assert.Equal(created, skill.CreatedAt);
        Assert.Null(skill.DeletedAt);
    }
}
