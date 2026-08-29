using IAS.Application.Common.Exceptions;
using IAS.Application.Skills;
using MediatR;

namespace IAS.Application.Skills.Queries.GetSkillById;

public sealed class GetSkillByIdQueryHandler(ISkillRepository repository)
    : IRequestHandler<GetSkillByIdQuery, SkillDto>
{
    public async Task<SkillDto> Handle(GetSkillByIdQuery request, CancellationToken cancellationToken)
    {
        var skill = await repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException($"Skill '{request.Id}' não encontrada.");

        return skill.ToDto();
    }
}
