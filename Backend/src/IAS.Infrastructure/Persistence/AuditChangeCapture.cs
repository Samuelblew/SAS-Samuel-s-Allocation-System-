using System.Text.Json;
using IAS.Application.Common.Interfaces;
using IAS.Domain.AuditLogs;
using IAS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace IAS.Infrastructure.Persistence;

internal sealed record PendingAuditEntry(
    string EntityType,
    Guid EntityId,
    Guid TenantId,
    AuditAction Action,
    string? ChangesJson);

internal static class AuditChangeCapture
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = false };

    private static readonly HashSet<string> IgnoredProperties =
    [
        nameof(TenantEntity.UpdatedAt),
        nameof(TenantEntity.DeletedAt)
    ];

    public static IReadOnlyList<PendingAuditEntry> Capture(
        ChangeTracker changeTracker,
        ITenantContext tenantContext)
    {
        if (!tenantContext.IsResolved)
            return [];

        var entries = new List<PendingAuditEntry>();

        foreach (var entry in changeTracker.Entries())
        {
            if (entry.Entity is AuditLog)
                continue;

            if (entry.Entity is not TenantEntity entity)
                continue;

            var pending = MapEntry(entry, entity);
            if (pending is not null)
                entries.Add(pending);
        }

        return entries;
    }

    public static AuditLog ToAuditLog(
        PendingAuditEntry pending,
        IAuditActorContext? actorContext)
    {
        var actorId = actorContext?.IsResolved == true ? actorContext.ActorId : null;
        var summary = $"{pending.Action} {pending.EntityType} {pending.EntityId}";

        return new AuditLog
        {
            Id = Guid.NewGuid(),
            TenantId = pending.TenantId,
            EntityType = pending.EntityType,
            EntityId = pending.EntityId,
            Action = pending.Action,
            ActorId = actorId,
            Summary = summary,
            ChangesJson = pending.ChangesJson,
            OccurredAt = DateTime.UtcNow
        };
    }

    private static PendingAuditEntry? MapEntry(EntityEntry entry, TenantEntity entity)
    {
        var entityType = entry.Entity.GetType().Name;
        var entityId = entity.Id;
        var tenantId = entity.TenantId;

        return entry.State switch
        {
            EntityState.Added => new PendingAuditEntry(
                entityType,
                entityId,
                tenantId,
                AuditAction.Created,
                SerializeProperties(entry, useCurrent: true)),

            EntityState.Deleted => new PendingAuditEntry(
                entityType,
                entityId,
                tenantId,
                AuditAction.Deleted,
                null),

            EntityState.Modified => MapModified(entry, entityType, entityId, tenantId),

            _ => null
        };
    }

    private static PendingAuditEntry? MapModified(
        EntityEntry entry,
        string entityType,
        Guid entityId,
        Guid tenantId)
    {
        var deletedAt = entry.Property(nameof(TenantEntity.DeletedAt));
        if (deletedAt.IsModified && deletedAt.CurrentValue is not null)
        {
            return new PendingAuditEntry(
                entityType,
                entityId,
                tenantId,
                AuditAction.Deleted,
                SerializeProperties(entry, useCurrent: true));
        }

        var changes = SerializeChanges(entry);
        if (changes is null)
            return null;

        return new PendingAuditEntry(
            entityType,
            entityId,
            tenantId,
            AuditAction.Updated,
            changes);
    }

    private static string? SerializeChanges(EntityEntry entry)
    {
        var changes = new Dictionary<string, object?>();

        foreach (var property in entry.Properties)
        {
            if (!property.IsModified || IgnoredProperties.Contains(property.Metadata.Name))
                continue;

            changes[property.Metadata.Name] = new
            {
                before = property.OriginalValue,
                after = property.CurrentValue
            };
        }

        return changes.Count == 0 ? null : JsonSerializer.Serialize(changes, JsonOptions);
    }

    private static string? SerializeProperties(EntityEntry entry, bool useCurrent)
    {
        var snapshot = new Dictionary<string, object?>();

        foreach (var property in entry.Properties)
        {
            if (property.Metadata.IsPrimaryKey())
                continue;

            snapshot[property.Metadata.Name] = useCurrent
                ? property.CurrentValue
                : property.OriginalValue;
        }

        return snapshot.Count == 0 ? null : JsonSerializer.Serialize(snapshot, JsonOptions);
    }
}
