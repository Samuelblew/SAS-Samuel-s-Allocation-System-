namespace IAS.Application.Common.Interfaces;

public interface IAuditActorContext
{
    string? ActorId { get; }
    string? ActorDisplayName { get; }
    bool IsResolved { get; }
}
