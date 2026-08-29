using IAS.Application.AllocationNeeds;
using IAS.Application.Capacity;
using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.Matching.Queries.GetAllocationNeedCandidates;

public sealed class GetAllocationNeedCandidatesQueryHandler(
    IAllocationNeedRepository needRepository,
    ICapacityReadRepository capacityRepository) : IRequestHandler<GetAllocationNeedCandidatesQuery, AllocationNeedCandidatesDto>
{
    public async Task<AllocationNeedCandidatesDto> Handle(
        GetAllocationNeedCandidatesQuery request,
        CancellationToken cancellationToken)
    {
        var need = await needRepository.GetByIdAsync(request.AllocationNeedId, cancellationToken)
            ?? throw new NotFoundException($"Necessidade de alocação '{request.AllocationNeedId}' não encontrada.");

        var (periodStart, periodEnd) = AllocationNeedCandidateMatcher.ResolvePeriod(need);
        var data = await AllocationNeedCandidateRanker.LoadCapacityDataAsync(
            capacityRepository,
            periodStart,
            periodEnd,
            cancellationToken);

        var filters = new CandidateMatchFilters(
            request.MinAvailablePercent,
            request.ExcludePeopleOnProject);

        return AllocationNeedCandidateRanker.Rank(
            need,
            data,
            request.MaxResults,
            filters);
    }
}
