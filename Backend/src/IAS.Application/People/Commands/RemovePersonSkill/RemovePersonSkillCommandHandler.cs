using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.People.Commands.RemovePersonSkill;

public sealed class RemovePersonSkillCommandHandler(IPersonRepository repository)
    : IRequestHandler<RemovePersonSkillCommand>
{
    public async Task Handle(RemovePersonSkillCommand request, CancellationToken cancellationToken)
    {
        var personSkill = await repository.GetPersonSkillAsync(request.PersonSkillId, cancellationToken)
            ?? throw new NotFoundException($"Skill da pessoa '{request.PersonSkillId}' não encontrada.");

        if (personSkill.PersonId != request.PersonId)
            throw new NotFoundException($"Skill da pessoa '{request.PersonSkillId}' não pertence à pessoa informada.");

        personSkill.DeletedAt = DateTime.UtcNow;
        personSkill.UpdatedAt = personSkill.DeletedAt;

        await repository.SaveChangesAsync(cancellationToken);
    }
}
