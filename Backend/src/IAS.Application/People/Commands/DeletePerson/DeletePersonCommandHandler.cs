using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.People.Commands.DeletePerson;

public sealed class DeletePersonCommandHandler(IPersonRepository repository)
    : IRequestHandler<DeletePersonCommand>
{
    public async Task Handle(DeletePersonCommand request, CancellationToken cancellationToken)
    {
        var person = await repository.GetByIdAsync(request.Id, includeSkills: true, cancellationToken)
            ?? throw new NotFoundException($"Pessoa '{request.Id}' não encontrada.");

        var now = DateTime.UtcNow;
        person.DeletedAt = now;
        person.UpdatedAt = now;

        foreach (var skill in person.Skills)
        {
            skill.DeletedAt = now;
            skill.UpdatedAt = now;
        }

        await repository.SaveChangesAsync(cancellationToken);
    }
}
