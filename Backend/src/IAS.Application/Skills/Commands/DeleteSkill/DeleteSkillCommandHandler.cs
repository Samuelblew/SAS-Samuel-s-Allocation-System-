using IAS.Application.Common.Exceptions;
using MediatR;

namespace IAS.Application.Skills.Commands.DeleteSkill;

public sealed class DeleteSkillCommandHandler(ISkillRepository repository)
    : IRequestHandler<DeleteSkillCommand>
{
    public async Task Handle(DeleteSkillCommand request, CancellationToken cancellationToken)
    {
        var skill = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Skill '{request.Id}' não encontrada.");

        skill.DeletedAt = DateTime.UtcNow;
        skill.UpdatedAt = skill.DeletedAt;

        await repository.SaveChangesAsync(cancellationToken);
    }
}
