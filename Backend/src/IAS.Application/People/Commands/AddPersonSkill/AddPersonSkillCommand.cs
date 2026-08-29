using IAS.Application.People;
using IAS.Domain.People;
using MediatR;

namespace IAS.Application.People.Commands.AddPersonSkill;

public sealed record AddPersonSkillCommand(
    Guid PersonId,
    Guid SkillId,
    SkillProficiencyLevel Level,
    DateTime? LastUsedAt,
    string? Notes) : IRequest<PersonSkillDto>;
