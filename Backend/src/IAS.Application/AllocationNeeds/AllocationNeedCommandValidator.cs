using FluentValidation;

namespace IAS.Application.AllocationNeeds;

public class AllocationNeedCommandValidator<T> : AbstractValidator<T>
    where T : IAllocationNeedCommand
{
    public AllocationNeedCommandValidator()
    {
        RuleFor(x => x.ProjectId).NotEmpty();
        RuleFor(x => x.Role).NotEmpty().MaximumLength(80);
        RuleFor(x => x.ExpectedSeniority).MaximumLength(80);
        RuleFor(x => x.DedicationPercent).GreaterThan(0).LessThanOrEqualTo(100);
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
        RuleFor(x => x.Urgency).IsInEnum();
        RuleFor(x => x.Criticality).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x)
            .Must(x => !x.RequiredSkillIds.Intersect(x.DesiredSkillIds).Any())
            .WithMessage("Uma skill não pode estar em obrigatórias e desejáveis ao mesmo tempo.");
    }
}
