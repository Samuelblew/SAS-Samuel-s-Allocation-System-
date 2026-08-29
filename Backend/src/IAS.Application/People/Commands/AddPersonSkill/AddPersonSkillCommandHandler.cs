using IAS.Application.Common.Exceptions;
using IAS.Application.Common.Interfaces;
using IAS.Application.People;
using IAS.Domain.People;
using MediatR;

namespace IAS.Application.People.Commands.AddPersonSkill;

public sealed class AddPersonSkillCommandHandler(
    IPersonRepository repository,
    ITenantContext tenantContext) : IRequestHandler<AddPersonSkillCommand, PersonSkillDto>
{
    public async Task<PersonSkillDto> Handle(AddPersonSkillCommand request, CancellationToken cancellationToken)
    {
        if (!tenantContext.IsResolved)
            throw new InvalidOperationException("Tenant não resolvido.");

        _ = await repository.GetByIdAsync(request.PersonId, includeSkills: false, cancellationToken)
            ?? throw new NotFoundException($"Pessoa '{request.PersonId}' não encontrada.");

        if (!await repository.SkillCatalogExistsAsync(request.SkillId, cancellationToken))
            throw new NotFoundException($"Skill '{request.SkillId}' não encontrada no catálogo.");

        if (await repository.SkillExistsForPersonAsync(request.PersonId, request.SkillId, cancellationToken: cancellationToken))
            throw new ConflictException("Esta pessoa já possui essa skill cadastrada.");

        var now = DateTime.UtcNow;
        var personSkill = new PersonSkill
        {
            Id = Guid.NewGuid(),
            TenantId = tenantContext.TenantId,
            PersonId = request.PersonId,
            SkillId = request.SkillId,
            Level = request.Level,
            LastUsedAt = request.LastUsedAt,
            Notes = string.IsNullOrWhiteSpace(request.Notes) ? null : request.Notes.Trim(),
            CreatedAt = now
        };

        await repository.AddPersonSkillAsync(personSkill, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        var loaded = await repository.GetPersonSkillAsync(personSkill.Id, cancellationToken)
            ?? throw new InvalidOperationException("Falha ao carregar PersonSkill criada.");

        return loaded.ToDto();
    }
}
