using IAS.Domain.AllocationNeeds;
using IAS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAS.Infrastructure.AllocationNeeds;

public sealed class AllocationNeedConfiguration : IEntityTypeConfiguration<AllocationNeed>
{
    public void Configure(EntityTypeBuilder<AllocationNeed> builder)
    {
        builder.ToTable("allocation_needs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.ProjectId).HasColumnName("project_id").IsRequired();
        builder.Property(x => x.Role).HasColumnName("role").HasMaxLength(80).IsRequired();
        builder.Property(x => x.ExpectedSeniority).HasColumnName("expected_seniority").HasMaxLength(80);
        builder.Property(x => x.DedicationPercent).HasColumnName("dedication_percent").HasPrecision(5, 2);
        builder.Property(x => x.StartDate).HasColumnName("start_date");
        builder.Property(x => x.EndDate).HasColumnName("end_date");
        builder.Property(x => x.Urgency).HasColumnName("urgency").HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.Criticality).HasColumnName("criticality").HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        var skillIdsConverter = JsonListGuidConverter.Create();
        var skillIdsComparer = JsonListGuidConverter.CreateComparer();

        builder.Property(x => x.RequiredSkillIds)
            .HasColumnName("required_skill_ids")
            .HasColumnType("json")
            .HasConversion(skillIdsConverter)
            .Metadata.SetValueComparer(skillIdsComparer);

        builder.Property(x => x.DesiredSkillIds)
            .HasColumnName("desired_skill_ids")
            .HasColumnType("json")
            .HasConversion(skillIdsConverter)
            .Metadata.SetValueComparer(skillIdsComparer);

        builder.HasOne(x => x.Project)
            .WithMany()
            .HasForeignKey(x => x.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ProjectId).HasDatabaseName("ix_allocation_needs_project_id");
    }
}
