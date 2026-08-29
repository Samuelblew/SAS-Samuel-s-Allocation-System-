using IAS.Domain.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAS.Infrastructure.Projects;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.ClientId).HasColumnName("client_id").IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.StartDate).HasColumnName("start_date");
        builder.Property(x => x.EndDate).HasColumnName("end_date");
        builder.Property(x => x.Priority).HasColumnName("priority").HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.Budget).HasColumnName("budget").HasPrecision(18, 2);
        builder.Property(x => x.EstimatedRevenue).HasColumnName("estimated_revenue").HasPrecision(18, 2);
        builder.Property(x => x.ProjectType).HasColumnName("project_type").HasMaxLength(80);
        builder.Property(x => x.CommercialOwner).HasColumnName("commercial_owner").HasMaxLength(120);
        builder.Property(x => x.DeliveryOwner).HasColumnName("delivery_owner").HasMaxLength(120);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        builder.HasIndex(x => x.ClientId).HasDatabaseName("ix_projects_client_id");
    }
}
