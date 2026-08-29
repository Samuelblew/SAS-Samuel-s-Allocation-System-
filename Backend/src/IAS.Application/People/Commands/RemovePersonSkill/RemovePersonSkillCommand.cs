using MediatR;

namespace IAS.Application.People.Commands.RemovePersonSkill;

public sealed record RemovePersonSkillCommand(Guid PersonId, Guid PersonSkillId) : IRequest;
