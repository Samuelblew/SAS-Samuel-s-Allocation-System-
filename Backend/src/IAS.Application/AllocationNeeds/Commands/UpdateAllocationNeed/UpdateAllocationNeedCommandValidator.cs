using FluentValidation;
using IAS.Application.AllocationNeeds;

namespace IAS.Application.AllocationNeeds.Commands.UpdateAllocationNeed;

public sealed class UpdateAllocationNeedCommandValidator : AbstractValidator<UpdateAllocationNeedCommand>
{
    public UpdateAllocationNeedCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        Include(new AllocationNeedCommandValidator<UpdateAllocationNeedCommand>());
    }
}
