using MediatR;

namespace IAS.Application.Skills.Commands.UpdateSkill;

public sealed record UpdateSkillCommand(Guid Id, string Name, string? Category) : IRequest<SkillDto>;
