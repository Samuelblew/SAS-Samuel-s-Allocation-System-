using IAS.Domain.AllocationNeeds;
using IAS.Domain.Common;
using IAS.Domain.People;

namespace IAS.Domain.Matching;

public class MatchingSuggestion : TenantEntity
{
    public Guid AllocationNeedId { get; set; }
    public AllocationNeed AllocationNeed { get; set; } = null!;
    public Guid PersonId { get; set; }
    public Person Person { get; set; } = null!;
    public MatchingSuggestionDecision Decision { get; set; }
    public decimal Score { get; set; }
    public string? Notes { get; set; }
    public Guid? DecidedByUserId { get; set; }
}
