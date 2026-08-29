using IAS.Domain.AuditLogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IAS.Infrastructure.AuditLogs;

public sealed class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id).HasColumnName("id");
        builder.Property(x => x.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(x => x.EntityType).HasColumnName("entity_type").HasMaxLength(80).IsRequired();
        builder.Property(x => x.EntityId).HasColumnName("entity_id").IsRequired();
        builder.Property(x => x.Action).HasColumnName("action").HasConversion<string>().HasMaxLength(32);
        builder.Property(x => x.ActorId).HasColumnName("actor_id").HasMaxLength(120);
        builder.Property(x => x.Summary).HasColumnName("summary").HasMaxLength(500);
        builder.Property(x => x.ChangesJson).HasColumnName("changes_json").HasColumnType("json");
        builder.Property(x => x.OccurredAt).HasColumnName("occurred_at").IsRequired();

        builder.HasIndex(x => new { x.TenantId, x.OccurredAt })
            .HasDatabaseName("ix_audit_logs_tenant_id_occurred_at");

        builder.HasIndex(x => new { x.TenantId, x.EntityType, x.EntityId })
            .HasDatabaseName("ix_audit_logs_tenant_id_entity");
    }
}
