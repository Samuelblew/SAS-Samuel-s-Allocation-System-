using FluentValidation;

namespace IAS.Application.Financial.Commands.SimulateAllocationMargin;

public sealed class SimulateAllocationMarginCommandValidator : AbstractValidator<SimulateAllocationMarginCommand>
{
    public SimulateAllocationMarginCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.PersonId).NotEmpty();
        RuleFor(x => x.Role).NotEmpty().MaximumLength(100);
        RuleFor(x => x.DedicationPercent).InclusiveBetween(1, 100);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
        RuleFor(x => x.MarginAlertThresholdPercent).InclusiveBetween(0, 100);
    }
}
