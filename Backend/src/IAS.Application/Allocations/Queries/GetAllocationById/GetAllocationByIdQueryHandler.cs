using IAS.Application.Allocations;
using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.Allocations.Queries.GetAllocationById;

public sealed class GetAllocationByIdQueryHandler(IAllocationRepository repository)
    : IRequestHandler<GetAllocationByIdQuery, AllocationDto>
{
    public async Task<AllocationDto> Handle(
        GetAllocationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Alocação '{request.Id}' não encontrada.");

        return entity.ToDto();
    }
}
