using IAS.Application.Allocations;
using MediatR;

namespace IAS.Application.Allocations.Queries.ListAllocationConflicts;

public sealed record ListAllocationConflictsQuery(
    Guid? PersonId = null,
    Guid? ProjectId = null,
    DateOnly? From = null,
    DateOnly? To = null) : IRequest<IReadOnlyList<AllocationConflictDto>>;
