using IAS.Domain.People;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAS.Infrastructure.People;

public sealed class PersonSkillConfiguration : IEntityTypeConfiguration<PersonSkill>
{
    public void Configure(EntityTypeBuilder<PersonSkill> builder)
    {
        builder.ToTable("person_skills");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.PersonId).HasColumnName("person_id").IsRequired();
        builder.Property(x => x.SkillId).HasColumnName("skill_id").IsRequired();
        builder.Property(x => x.Level).HasColumnName("level").HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.LastUsedAt).HasColumnName("last_used_at");
        builder.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(500);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        builder.HasOne(x => x.Skill)
            .WithMany()
            .HasForeignKey(x => x.SkillId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.PersonId, x.SkillId })
            .HasDatabaseName("ix_person_skills_person_id_skill_id");
    }
}
