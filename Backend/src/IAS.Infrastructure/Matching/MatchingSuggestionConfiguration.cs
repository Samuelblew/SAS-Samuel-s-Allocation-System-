using IAS.Domain.Matching;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAS.Infrastructure.Matching;

public sealed class MatchingSuggestionConfiguration : IEntityTypeConfiguration<MatchingSuggestion>
{
    public void Configure(EntityTypeBuilder<MatchingSuggestion> builder)
    {
        builder.ToTable("matching_suggestions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.AllocationNeedId).HasColumnName("allocation_need_id").IsRequired();
        builder.Property(x => x.PersonId).HasColumnName("person_id").IsRequired();
        builder.Property(x => x.Decision).HasColumnName("decision").HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.Score).HasColumnName("score").HasPrecision(8, 2);
        builder.Property(x => x.Notes).HasColumnName("notes").HasMaxLength(500);
        builder.Property(x => x.DecidedByUserId).HasColumnName("decided_by_user_id");
        builder.Property(x => x.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(x => x.UpdatedAt).HasColumnName("updated_at");
        builder.Property(x => x.DeletedAt).HasColumnName("deleted_at");

        builder.HasOne(x => x.AllocationNeed)
            .WithMany()
            .HasForeignKey(x => x.AllocationNeedId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Person)
            .WithMany()
            .HasForeignKey(x => x.PersonId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.AllocationNeedId).HasDatabaseName("ix_matching_suggestions_allocation_need_id");
        builder.HasIndex(x => new { x.TenantId, x.CreatedAt }).HasDatabaseName("ix_matching_suggestions_tenant_created");
    }
}
