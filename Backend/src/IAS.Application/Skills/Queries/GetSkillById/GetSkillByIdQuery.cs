using MediatR;

namespace IAS.Application.Skills.Queries.GetSkillById;

public sealed record GetSkillByIdQuery(Guid Id) : IRequest<SkillDto>;
