using IAS.Application.Common.Exceptions;
using IAS.Application.People;
using MediatR;

namespace IAS.Application.People.Commands.UpdatePersonSkill;

public sealed class UpdatePersonSkillCommandHandler(IPersonRepository repository)
    : IRequestHandler<UpdatePersonSkillCommand, PersonSkillDto>
{
    public async Task<PersonSkillDto> Handle(UpdatePersonSkillCommand request, CancellationToken cancellationToken)
    {
        var personSkill = await repository.GetPersonSkillAsync(request.PersonSkillId, cancellationToken)
            ?? throw new NotFoundException($"Skill da pessoa '{request.PersonSkillId}' não encontrada.");

        if (personSkill.PersonId != request.PersonId)
            throw new NotFoundException($"Skill da pessoa '{request.PersonSkillId}' não pertence à pessoa informada.");

        personSkill.Level = request.Level;
        personSkill.LastUsedAt = request.LastUsedAt;
        personSkill.Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim();
        personSkill.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync(cancellationToken);

        return personSkill.ToDto();
    }
}
