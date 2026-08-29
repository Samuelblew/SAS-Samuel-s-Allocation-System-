using MediatR;

namespace IAS.Application.People.Commands.DeletePerson;

public sealed record DeletePersonCommand(Guid Id) : IRequest;
