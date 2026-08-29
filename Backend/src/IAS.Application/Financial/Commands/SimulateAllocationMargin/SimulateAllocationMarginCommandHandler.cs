using IAS.Application.Common.Exceptions;
using IAS.Application.Capacity;
using MediatR;

namespace IAS.Application.Financial.Commands.SimulateAllocationMargin;

public sealed class SimulateAllocationMarginCommandHandler(
    IFinancialReadRepository financialRepository,
    ICapacityReadRepository capacityRepository) : IRequestHandler<SimulateAllocationMarginCommand, AllocationMarginSimulationDto>
{
    public async Task<AllocationMarginSimulationDto> Handle(
        SimulateAllocationMarginCommand request,
        CancellationToken cancellationToken)
    {
        var project = await financialRepository.GetProjectWithClientAsync(request.ProjectId, cancellationToken)
            ?? throw new NotFoundException($"Projeto '{request.ProjectId}' não encontrado.");

        var person = await capacityRepository.GetPersonAsync(request.PersonId, cancellationToken)
            ?? throw new NotFoundException($"Pessoa '{request.PersonId}' não encontrada.");

        var allocations = await financialRepository.GetProjectAllocationsWithPeopleAsync(
            request.ProjectId,
            cancellationToken);

        var result = AllocationMarginSimulator.Simulate(
            project,
            allocations,
            person,
            request.Role,
            request.DedicationPercent,
            request.StartDate,
            request.EndDate,
            request.MarginAlertThresholdPercent);

        return Map(result);
    }

    private static AllocationMarginSimulationDto Map(AllocationMarginSimulationResult r) =>
        new(
            r.ProjectId,
            r.ProjectName,
            r.PeriodStart,
            r.PeriodEnd,
            r.CurrentTotalCost,
            r.CurrentMarginAmount,
            r.CurrentMarginPercent,
            r.SimulatedAdditionalCost,
            r.ProjectedTotalCost,
            r.ProjectedMarginAmount,
            r.ProjectedMarginPercent,
            r.MarginDeltaAmount,
            r.MarginDeltaPercent,
            r.HasRevenueData,
            r.CurrentIsLowMarginAlert,
            r.ProjectedIsLowMarginAlert,
            r.MarginAlertThresholdPercent);
}
