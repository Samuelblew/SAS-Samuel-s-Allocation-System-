using IAS.Application.Allocations;
using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.Allocations.Queries.GetPersonProjectsView;

public sealed class GetPersonProjectsViewQueryHandler(IAllocationRepository repository)
    : IRequestHandler<GetPersonProjectsViewQuery, PersonProjectsViewDto>
{
    public async Task<PersonProjectsViewDto> Handle(
        GetPersonProjectsViewQuery request,
        CancellationToken cancellationToken)
    {
        var person = await repository.GetPersonAsync(request.PersonId, cancellationToken)
            ?? throw new NotFoundException($"Pessoa '{request.PersonId}' não encontrada.");

        var allocations = await repository.GetByPersonIdAsync(request.PersonId, cancellationToken);
        return AllocationViewMapping.ToPersonProjectsView(allocations, person.Id, person.Name);
    }
}
