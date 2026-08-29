using IAS.Application.Common.Exceptions;
using IAS.Application.Common.Interfaces;
using IAS.Domain.Allocations;
using IAS.Domain.People;

namespace IAS.Application.Allocations;

internal static class AllocationCommandHandlerBase
{
    private static readonly PersonStatus[] InactiveStatuses =
    [
        PersonStatus.Offboarded,
        PersonStatus.NoticePeriod
    ];

    public static async Task ValidateAsync(
        IAllocationCommand command,
        IAllocationRepository repository,
        ITenantContext tenantContext,
        Guid? excludeAllocationId,
        CancellationToken cancellationToken)
    {
        if (!tenantContext.IsResolved)
            throw new InvalidOperationException("Tenant não resolvido.");

        var person = await repository.GetPersonAsync(command.PersonId, cancellationToken)
            ?? throw new NotFoundException($"Pessoa '{command.PersonId}' não encontrada.");

        if (InactiveStatuses.Contains(person.Status))
            throw new ConflictException("Pessoa inativa não pode receber nova alocação.");

        if (!await repository.ProjectExistsAsync(command.ProjectId, cancellationToken))
            throw new NotFoundException($"Projeto '{command.ProjectId}' não encontrado.");

        var overlapping = await repository.GetOverlappingForPersonAsync(
            command.PersonId,
            command.StartDate,
            command.EndDate,
            excludeAllocationId,
            cancellationToken);

        if (AllocationOverloadChecker.WouldExceedWeeklyCapacity(
                command.StartDate,
                command.EndDate,
                command.DedicationPercent,
                overlapping,
                excludeAllocationId))
        {
            var overload = AllocationOverloadChecker.FindFirstOverloadWeek(
                command.StartDate,
                command.EndDate,
                command.DedicationPercent,
                overlapping,
                excludeAllocationId);

            if (overload is null)
            {
                throw new ConflictException(
                    "A alocação ultrapassa 100% de dedicação em pelo menos uma semana (RN-001).");
            }

            var existingSummary = string.Join(
                ", ",
                overload.ExistingAllocations.Select(a => $"{a.ProjectName} ({a.DedicationPercent:G29}%)"));

            throw new ConflictException(
                $"RN-001: na semana de {overload.WeekStart:dd/MM/yyyy} a {overload.WeekEnd:dd/MM/yyyy}, " +
                $"a pessoa já tem {overload.ExistingPercent:G29}% alocados ({existingSummary}). " +
                $"Com mais {overload.RequestedPercent:G29}%, o total seria {overload.TotalPercent:G29}%.");
        }
    }

    public static void ApplyToEntity(Allocation entity, IAllocationCommand command)
    {
        entity.PersonId = command.PersonId;
        entity.ProjectId = command.ProjectId;
        entity.Role = command.Role.Trim();
        entity.DedicationPercent = command.DedicationPercent;
        entity.StartDate = command.StartDate;
        entity.EndDate = command.EndDate;
        entity.Status = command.Status;
        entity.Notes = string.IsNullOrWhiteSpace(command.Notes) ? null : command.Notes.Trim();
    }
}
