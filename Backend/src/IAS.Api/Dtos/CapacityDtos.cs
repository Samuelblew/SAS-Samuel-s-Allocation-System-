using IAS.Domain.AllocationNeeds;
using IAS.Domain.Projects;

namespace IAS.Api.Dtos;

public sealed record PersonAvailabilityResponse(
    Guid PersonId,
    string PersonName,
    decimal WeeklyCapacityHours,
    DateOnly From,
    DateOnly To,
    IReadOnlyList<WeekAvailabilityResponse> Weeks);

public sealed record WeekAvailabilityResponse(
    DateOnly WeekStart,
    DateOnly WeekEnd,
    decimal AllocatedPercent,
    decimal AvailablePercent,
    decimal WeeklyCapacityHours,
    decimal AllocatedHours,
    decimal AvailableHours,
    bool IsUnavailable);

public sealed record ProjectStaffingGapsResponse(
    Guid ProjectId,
    string ProjectName,
    IReadOnlyList<StaffingGapItemResponse> Needs);

public sealed record StaffingGapItemResponse(
    Guid NeedId,
    string Role,
    decimal RequiredPercent,
    decimal CoveredPercent,
    decimal GapPercent,
    AllocationNeedStatus Status);

public sealed record AvailablePeopleResponse(
    DateOnly From,
    DateOnly To,
    IReadOnlyList<AvailablePersonResponse> People);

public sealed record AvailablePersonResponse(
    Guid PersonId,
    string PersonName,
    decimal MinAvailablePercentInPeriod,
    decimal AvgAvailablePercent);

public sealed record CapacityOverviewResponse(
    DateOnly From,
    DateOnly To,
    IReadOnlyList<WeekOverviewResponse> Weeks,
    IReadOnlyList<TeamOccupationResponse> Teams);

public sealed record WeekOverviewResponse(
    DateOnly WeekStart,
    DateOnly WeekEnd,
    int ActivePeopleCount,
    decimal AvgAllocatedPercent,
    decimal AvgAvailablePercent,
    int BenchPeopleCount,
    int OverallocatedPeopleCount,
    decimal TotalCapacityHours,
    decimal TotalAllocatedHours,
    decimal TotalAvailableHours);

public sealed record SkillsOccupationResponse(
    DateOnly From,
    DateOnly To,
    IReadOnlyList<SkillOccupationResponse> Skills);

public sealed record FutureCapacityGapsResponse(
    DateOnly From,
    DateOnly To,
    decimal PeakShortfallPercent,
    IReadOnlyList<WeekCapacityGapResponse> Weeks,
    IReadOnlyList<OpenNeedGapResponse> OpenNeeds);

public sealed record WeekCapacityGapResponse(
    DateOnly WeekStart,
    DateOnly WeekEnd,
    decimal TotalGapDemandPercent,
    decimal TotalAvailableSupplyPercent,
    decimal NetShortfallPercent,
    int OpenNeedsInWeek);

public sealed record OpenNeedGapResponse(
    Guid NeedId,
    Guid ProjectId,
    string ProjectName,
    string Role,
    decimal RequiredPercent,
    decimal CoveredPercent,
    decimal GapPercent,
    AllocationNeedStatus Status,
    DateOnly? StartDate,
    DateOnly? EndDate);

public sealed record SkillOccupationResponse(
    Guid SkillId,
    string SkillName,
    string? Category,
    int PeopleCount,
    decimal AvgAllocatedPercent,
    decimal AvgAvailablePercent,
    decimal AvgAllocatedHours,
    decimal AvgAvailableHours);

public sealed record TeamOccupationResponse(
    string? Team,
    int PeopleCount,
    decimal AvgAllocatedPercent,
    decimal AvgAvailablePercent);

public sealed record BenchPeopleResponse(
    DateOnly From,
    DateOnly To,
    decimal MinAvailablePercent,
    IReadOnlyList<BenchPersonResponse> People);

public sealed record BenchPersonResponse(
    Guid PersonId,
    string PersonName,
    string? Team,
    string? Seniority,
    decimal MinAvailablePercentInPeriod,
    decimal AvgAvailablePercent);

public sealed record UnderstaffedProjectsResponse(
    IReadOnlyList<UnderstaffedProjectResponse> Items);

public sealed record UnderstaffedProjectResponse(
    Guid ProjectId,
    string ProjectName,
    ProjectStatus Status,
    int OpenNeedsCount,
    decimal TotalGapPercent);
