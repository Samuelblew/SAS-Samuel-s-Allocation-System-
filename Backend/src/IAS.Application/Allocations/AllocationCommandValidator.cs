using FluentValidation;

namespace IAS.Application.Allocations;

public class AllocationCommandValidator<T> : AbstractValidator<T>
    where T : IAllocationCommand
{
    public AllocationCommandValidator()
    {
        RuleFor(x => x.PersonId).NotEmpty();
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Role).NotEmpty().MaximumLength(80);
        RuleFor(x => x.DedicationPercent).GreaterThan(0).LessThanOrEqualTo(100);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Notes).MaximumLength(500);
    }
}
