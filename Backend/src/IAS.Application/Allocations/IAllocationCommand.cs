using IAS.Domain.Allocations;

namespace IAS.Application.Allocations;

public interface IAllocationCommand
{
    Guid PersonId { get; }
    Guid ProjectId { get; }
    string Role { get; }
    decimal DedicationPercent { get; }
    DateOnly StartDate { get; }
    DateOnly EndDate { get; }
    AllocationStatus Status { get; }
    string? Notes { get; }
}
