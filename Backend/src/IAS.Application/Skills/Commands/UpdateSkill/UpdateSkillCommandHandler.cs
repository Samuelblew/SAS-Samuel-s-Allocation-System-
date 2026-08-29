using IAS.Application.Common.Exceptions;
using IAS.Application.Skills;
using MediatR;

namespace IAS.Application.Skills.Commands.UpdateSkill;

public sealed class UpdateSkillCommandHandler(ISkillRepository repository)
    : IRequestHandler<UpdateSkillCommand, SkillDto>
{
    public async Task<SkillDto> Handle(UpdateSkillCommand request, CancellationToken cancellationToken)
    {
        var skill = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Skill '{request.Id}' não encontrada.");

        if (await repository.ExistsByNameAsync(request.Name.Trim(), request.Id, cancellationToken))
            throw new ConflictException($"Já existe uma skill com o nome '{request.Name}'.");

        skill.Name = request.Name.Trim();
        skill.Category = string.IsNullOrWhiteSpace(request.Category) ? null : request.Category.Trim();
        skill.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync(cancellationToken);

        return skill.ToDto();
    }
}
