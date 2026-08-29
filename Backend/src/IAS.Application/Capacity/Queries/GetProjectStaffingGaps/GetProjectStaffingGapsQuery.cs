using IAS.Domain.AllocationNeeds;
using MediatR;

namespace IAS.Application.Capacity.Queries.GetProjectStaffingGaps;

public sealed record GetProjectStaffingGapsQuery(Guid ProjectId) : IRequest<ProjectStaffingGapsDto>;

public sealed record ProjectStaffingGapsDto(
    Guid ProjectId,
    string ProjectName,
    IReadOnlyList<StaffingGapItemDto> Needs);

public sealed record StaffingGapItemDto(
    Guid NeedId,
    string Role,
    decimal RequiredPercent,
    decimal CoveredPercent,
    decimal GapPercent,
    AllocationNeedStatus Status);
