using IAS.Application.AllocationNeeds;
using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.AllocationNeeds.Queries.GetAllocationNeedById;

public sealed class GetAllocationNeedByIdQueryHandler(IAllocationNeedRepository repository)
    : IRequestHandler<GetAllocationNeedByIdQuery, AllocationNeedDto>
{
    public async Task<AllocationNeedDto> Handle(
        GetAllocationNeedByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Necessidade de alocação '{request.Id}' não encontrada.");

        return entity.ToDto();
    }
}
