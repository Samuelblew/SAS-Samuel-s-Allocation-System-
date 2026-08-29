using IAS.Application.Common.Exceptions;
using IAS.Application.People;
using MediatR;

namespace IAS.Application.People.Queries.GetPersonById;

public sealed class GetPersonByIdQueryHandler(IPersonRepository repository)
    : IRequestHandler<GetPersonByIdQuery, PersonDto>
{
    public async Task<PersonDto> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var person = await repository.GetByIdAsync(request.Id, includeSkills: true, cancellationToken)
            ?? throw new NotFoundException($"Pessoa '{request.Id}' não encontrada.");

        return person.ToDto();
    }
}
