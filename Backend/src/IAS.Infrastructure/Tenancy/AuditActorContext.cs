using IAS.Application.Common.Interfaces;

namespace IAS.Infrastructure.Tenancy;

public sealed class AuditActorContext : IAuditActorContext
{
    public string? ActorId { get; private set; }
    public string? ActorDisplayName { get; private set; }
    public bool IsResolved { get; private set; }

    public void SetActor(string? actorId, string? displayName = null)
    {
        ActorId = string.IsNullOrWhiteSpace(actorId) ? null : actorId.Trim();
        ActorDisplayName = string.IsNullOrWhiteSpace(displayName) ? null : displayName.Trim();
        IsResolved = ActorId is not null;
    }
}
