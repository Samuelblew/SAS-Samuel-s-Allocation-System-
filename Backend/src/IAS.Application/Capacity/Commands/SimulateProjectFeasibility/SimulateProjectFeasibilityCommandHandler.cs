using MediatR;

namespace IAS.Application.Capacity.Commands.SimulateProjectFeasibility;

public sealed class SimulateProjectFeasibilityCommandHandler(ICapacityReadRepository repository)
    : IRequestHandler<SimulateProjectFeasibilityCommand, ProjectFeasibilityDto>
{
    public async Task<ProjectFeasibilityDto> Handle(
        SimulateProjectFeasibilityCommand request,
        CancellationToken cancellationToken)
    {
        var endDate = request.DesiredStartDate.AddMonths(request.DurationMonths);
        var data = await CapacityDataLoader.LoadAsync(
            repository, request.DesiredStartDate, endDate, includeSkills: true, cancellationToken);

        var needs = request.Needs.Select(n => new SimulatedNeed(
            n.Role.Trim(),
            string.IsNullOrWhiteSpace(n.ExpectedSeniority) ? null : n.ExpectedSeniority.Trim(),
            n.RequiredSkillIds,
            n.DedicationPercent,
            n.Quantity)).ToList();

        var result = ProjectFeasibilitySimulator.Simulate(
            request.DesiredStartDate,
            request.DurationMonths,
            needs,
            data);

        return new ProjectFeasibilityDto(
            result.DesiredStartDate,
            result.SimulatedEndDate,
            result.FeasibleAtDesiredStart,
            result.EarliestFeasibleStart,
            result.WeeksScanned,
            result.ActivePeopleCount,
            result.BenchAtDesiredStart,
            result.TotalHeadcountRequired,
            result.Roles.Select(r => new RoleFeasibilityDto(
                r.Role,
                r.ExpectedSeniority,
                r.DedicationPercent,
                r.QuantityRequired,
                r.CandidatesAtDesiredStart,
                r.SatisfiedAtDesiredStart,
                r.EligibleCandidates.Select(c => new RoleCandidatePreviewDto(
                    c.PersonId,
                    c.PersonName,
                    c.Seniority,
                    c.MinAvailablePercent)).ToList())).ToList());
    }
}
