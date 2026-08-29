using IAS.Application.AllocationNeeds;
using IAS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace IAS.Infrastructure.AllocationNeeds;

public sealed class AllocationNeedStatusSync(IasDbContext context) : IAllocationNeedStatusSync
{
    public async Task SyncForProjectAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        var needs = await context.AllocationNeeds
            .Where(n => n.ProjectId == projectId)
            .ToListAsync(cancellationToken);

        if (needs.Count == 0)
            return;

        var allocations = await context.Allocations
            .Where(a => a.ProjectId == projectId)
            .ToListAsync(cancellationToken);

        var changed = false;
        foreach (var need in needs)
        {
            var newStatus = AllocationNeedStatusCalculator.Calculate(need, allocations);
            if (need.Status == newStatus)
                continue;

            need.Status = newStatus;
            need.UpdatedAt = DateTime.UtcNow;
            changed = true;
        }

        if (changed)
            await context.SaveChangesAsync(cancellationToken);
    }
}
