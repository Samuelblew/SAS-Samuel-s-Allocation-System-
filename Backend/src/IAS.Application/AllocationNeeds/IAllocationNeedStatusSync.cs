namespace IAS.Application.AllocationNeeds;

public interface IAllocationNeedStatusSync
{
    Task SyncForProjectAsync(Guid projectId, CancellationToken cancellationToken = default);
}
