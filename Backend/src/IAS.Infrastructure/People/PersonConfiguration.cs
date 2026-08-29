using IAS.Domain.People;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAS.Infrastructure.People;

public sealed class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("people");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.Name).HasColumnName("name").HasMaxLength(200).IsRequired();
        builder.Property(x => x.JobTitle).HasColumnName("job_title").HasMaxLength(120);
        builder.Property(x => x.Seniority).HasColumnName("seniority").HasMaxLength(80);
        builder.Property(x => x.HourlyCost).HasColumnName("hourly_cost").HasPrecision(18, 2);
        builder.Property(x => x.MonthlyCost).HasColumnName("monthly_cost").HasPrecision(18, 2);
        builder.Property(x => x.WeeklyCapacityHours).HasColumnName("weekly_capacity_hours").HasPrecision(5, 2);
        builder.Property(x => x.Location).HasColumnName("location").HasMaxLength(120);
        builder.Property(x => x.Team).HasColumnName("team").HasMaxLength(120);
        builder.Property(x => x.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        builder.HasMany(x => x.Skills)
            .WithOne(x => x.Person)
            .HasForeignKey(x => x.PersonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
