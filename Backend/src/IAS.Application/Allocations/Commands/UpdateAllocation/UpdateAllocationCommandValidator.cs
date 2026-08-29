using FluentValidation;
using IAS.Application.Allocations;

namespace IAS.Application.Allocations.Commands.UpdateAllocation;

public sealed class UpdateAllocationCommandValidator : AbstractValidator<UpdateAllocationCommand>
{
    public UpdateAllocationCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        Include(new AllocationCommandValidator<UpdateAllocationCommand>());
    }
}
