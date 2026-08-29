using IAS.Application.Common.Exceptions;
using IAS.Application.Common.Interfaces;
using IAS.Application.Skills;
using IAS.Domain.Skills;
using MediatR;

namespace IAS.Application.Skills.Commands.CreateSkill;

public sealed class CreateSkillCommandHandler(
    ISkillRepository repository,
    ITenantContext tenantContext) : IRequestHandler<CreateSkillCommand, SkillDto>
{
    public async Task<SkillDto> Handle(CreateSkillCommand request, CancellationToken cancellationToken)
    {
        if (!tenantContext.IsResolved)
            throw new InvalidOperationException("Tenant não resolvido.");

        if (await repository.ExistsByNameAsync(request.Name.Trim(), cancellationToken: cancellationToken))
            throw new ConflictException($"Já existe uma skill com o nome '{request.Name}'.");

        var now = DateTime.UtcNow;
        var skill = new Skill
        {
            Id = Guid.NewGuid(),
            TenantId = tenantContext.TenantId,
            Name = request.Name.Trim(),
            Category = string.IsNullOrWhiteSpace(request.Category) ? null : request.Category.Trim(),
            CreatedAt = now
        };

        await repository.AddAsync(skill, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return skill.ToDto();
    }
}
