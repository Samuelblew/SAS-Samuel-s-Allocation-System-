using MediatR;

namespace IAS.Application.Skills.Commands.CreateSkill;

public sealed record CreateSkillCommand(string Name, string? Category) : IRequest<SkillDto>;
