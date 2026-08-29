using FluentValidation;

namespace IAS.Application.Capacity.Commands.SimulateProjectFeasibility;

public sealed class SimulateProjectFeasibilityCommandValidator : AbstractValidator<SimulateProjectFeasibilityCommand>
{
    public SimulateProjectFeasibilityCommandValidator()
    {
        RuleFor(x => x.DurationMonths).InclusiveBetween(1, 36);
        RuleFor(x => x.Needs).NotEmpty();

        RuleForEach(x => x.Needs).ChildRules(need =>
        {
            need.RuleFor(n => n.Role).NotEmpty().MaximumLength(100);
            need.RuleFor(n => n.DedicationPercent).InclusiveBetween(1, 100);
            need.RuleFor(n => n.Quantity).InclusiveBetween(1, 20);
        });
    }
}
