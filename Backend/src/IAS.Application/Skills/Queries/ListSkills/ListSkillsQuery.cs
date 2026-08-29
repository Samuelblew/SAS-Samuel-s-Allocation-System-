using IAS.Application.Common.Models;
using MediatR;

namespace IAS.Application.Skills.Queries.ListSkills;

public sealed record ListSkillsQuery(int Page = 1, int PageSize = 20) : IRequest<PagedResult<SkillDto>>;
