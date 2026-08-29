using IAS.Application.Common.Exceptions;
using IAS.Application.Common.Interfaces;
using IAS.Domain.AllocationNeeds;

namespace IAS.Application.AllocationNeeds;

internal static class AllocationNeedCommandHandlerBase
{
    public static async Task ValidateAsync(
        IAllocationNeedCommand command,
        IAllocationNeedRepository repository,
        ITenantContext tenantContext,
        CancellationToken cancellationToken)
    {
        if (!tenantContext.IsResolved)
            throw new InvalidOperationException("Tenant não resolvido.");

        if (!await repository.ProjectExistsAsync(command.ProjectId, cancellationToken))
            throw new NotFoundException($"Projeto '{command.ProjectId}' não encontrado.");

        var allSkillIds = command.RequiredSkillIds
            .Concat(command.DesiredSkillIds)
            .Distinct()
            .ToList();

        if (allSkillIds.Count > 0
            && !await repository.AllSkillsExistAsync(allSkillIds, cancellationToken))
            throw new NotFoundException("Uma ou mais skills informadas não existem no catálogo.");
    }

    public static void ApplyToEntity(AllocationNeed entity, IAllocationNeedCommand command)
    {
        entity.ProjectId = command.ProjectId;
        entity.Role = command.Role.Trim();
        entity.ExpectedSeniority = string.IsNullOrWhiteSpace(command.ExpectedSeniority)
            ? null
            : command.ExpectedSeniority.Trim();
        entity.RequiredSkillIds = command.RequiredSkillIds.Distinct().ToList();
        entity.DesiredSkillIds = command.DesiredSkillIds.Distinct().ToList();
        entity.DedicationPercent = command.DedicationPercent;
        entity.StartDate = command.StartDate;
        entity.EndDate = command.EndDate;
        entity.Urgency = command.Urgency;
        entity.Criticality = command.Criticality;
        entity.Status = command.Status;
    }
}
