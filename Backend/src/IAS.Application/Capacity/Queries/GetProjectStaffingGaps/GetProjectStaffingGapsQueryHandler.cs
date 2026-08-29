using IAS.Application.AllocationNeeds;
using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.Capacity.Queries.GetProjectStaffingGaps;

public sealed class GetProjectStaffingGapsQueryHandler(ICapacityReadRepository repository)
    : IRequestHandler<GetProjectStaffingGapsQuery, ProjectStaffingGapsDto>
{
    public async Task<ProjectStaffingGapsDto> Handle(
        GetProjectStaffingGapsQuery request,
        CancellationToken cancellationToken)
    {
        var project = await repository.GetProjectAsync(request.ProjectId, cancellationToken)
            ?? throw new NotFoundException($"Projeto '{request.ProjectId}' não encontrado.");

        var needs = await repository.GetNeedsForProjectAsync(request.ProjectId, cancellationToken);
        var allocations = await repository.GetAllocationsForProjectAsync(request.ProjectId, cancellationToken);
        var projectName = project.Name;

        var items = needs.Select(need =>
        {
            var covered = AllocationNeedStatusCalculator.CalculateCoveredPercent(need, allocations);
            var status = AllocationNeedStatusCalculator.ResolveStatus(covered, need.DedicationPercent);
            var gap = Math.Max(0, need.DedicationPercent - covered);

            return new StaffingGapItemDto(
                need.Id,
                need.Role,
                need.DedicationPercent,
                covered,
                gap,
                status);
        }).ToList();

        return new ProjectStaffingGapsDto(request.ProjectId, projectName, items);
    }
}
