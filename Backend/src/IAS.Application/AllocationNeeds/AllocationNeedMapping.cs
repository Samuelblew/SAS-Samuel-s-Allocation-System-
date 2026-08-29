using IAS.Domain.AllocationNeeds;

namespace IAS.Application.AllocationNeeds;

internal static class AllocationNeedMapping
{
    public static AllocationNeedDto ToDto(this AllocationNeed entity) =>
        new(
            entity.Id,
            entity.ProjectId,
            entity.Project.Name,
            entity.Role,
            entity.ExpectedSeniority,
            entity.RequiredSkillIds,
            entity.DesiredSkillIds,
            entity.DedicationPercent,
            entity.StartDate,
            entity.EndDate,
            entity.Urgency,
            entity.Criticality,
            entity.Status,
            entity.CreatedAt,
            entity.UpdatedAt);
}
