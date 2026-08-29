using IAS.Application.People;
using MediatR;

namespace IAS.Application.People.Queries.GetPersonById;

public sealed record GetPersonByIdQuery(Guid Id) : IRequest<PersonDto>;
