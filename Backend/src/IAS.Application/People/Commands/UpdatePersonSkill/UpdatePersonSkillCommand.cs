using IAS.Application.People;
using IAS.Domain.People;
using MediatR;

namespace IAS.Application.People.Commands.UpdatePersonSkill;

public sealed record UpdatePersonSkillCommand(
    Guid PersonId,
    Guid PersonSkillId,
    SkillProficiencyLevel Level,
    DateTime? LastUsedAt,
    string? Notes) : IRequest<PersonSkillDto>;
