using IAS.Application.Common.Models;
using IAS.Application.Skills;
using MediatR;

namespace IAS.Application.Skills.Queries.ListSkills;

public sealed class ListSkillsQueryHandler(ISkillRepository repository)
    : IRequestHandler<ListSkillsQuery, PagedResult<SkillDto>>
{
    public async Task<PagedResult<SkillDto>> Handle(ListSkillsQuery request, CancellationToken cancellationToken)
    {
        var (items, total) = await repository.ListAsync(request.Page, request.PageSize, cancellationToken);

        return new PagedResult<SkillDto>(
            items.Select(s => s.ToDto()).ToList(),
            request.Page,
            request.PageSize,
            total);
    }
}
