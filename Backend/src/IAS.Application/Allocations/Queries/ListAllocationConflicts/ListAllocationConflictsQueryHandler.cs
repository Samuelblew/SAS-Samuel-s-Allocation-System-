using IAS.Application.Allocations;
using MediatR;

namespace IAS.Application.Allocations.Queries.ListAllocationConflicts;

public sealed class ListAllocationConflictsQueryHandler(IAllocationRepository repository)
    : IRequestHandler<ListAllocationConflictsQuery, IReadOnlyList<AllocationConflictDto>>
{
    public async Task<IReadOnlyList<AllocationConflictDto>> Handle(
        ListAllocationConflictsQuery request,
        CancellationToken cancellationToken)
    {
        var allocations = await repository.GetActiveForConflictScanAsync(
            request.PersonId,
            request.ProjectId,
            request.From,
            request.To,
            cancellationToken);

        return AllocationOverloadChecker.DetectWeeklyConflicts(allocations);
    }
}
