using FluentValidation;

namespace IAS.Application.Projects.Commands.UpdateProject;

public sealed class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Priority).IsInEnum();
        RuleFor(x => x.EndDate).GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue);
        RuleFor(x => x.Budget).GreaterThanOrEqualTo(0).When(x => x.Budget.HasValue);
        RuleFor(x => x.EstimatedRevenue).GreaterThanOrEqualTo(0).When(x => x.EstimatedRevenue.HasValue);
        RuleFor(x => x.ProjectType).MaximumLength(80);
        RuleFor(x => x.CommercialOwner).MaximumLength(120);
        RuleFor(x => x.DeliveryOwner).MaximumLength(120);
    }
}
