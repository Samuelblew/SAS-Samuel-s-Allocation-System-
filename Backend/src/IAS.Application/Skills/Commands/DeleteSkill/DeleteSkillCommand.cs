using MediatR;

namespace IAS.Application.Skills.Commands.DeleteSkill;

public sealed record DeleteSkillCommand(Guid Id) : IRequest;
