using MediatR;

namespace IAS.Application.Capacity.Queries.GetSkillsOccupation;

public sealed class GetSkillsOccupationQueryHandler(ICapacityReadRepository repository)
    : IRequestHandler<GetSkillsOccupationQuery, SkillsOccupationDto>
{
    public async Task<SkillsOccupationDto> Handle(
        GetSkillsOccupationQuery request,
        CancellationToken cancellationToken)
    {
        var data = await CapacityDataLoader.LoadAsync(
            repository,
            request.From,
            request.To,
            includeSkills: true,
            cancellationToken);

        var skills = SkillOccupationCalculator.Calculate(request.From, request.To, data);

        return new SkillsOccupationDto(
            request.From,
            request.To,
            skills.Select(s => new SkillOccupationItemDto(
                s.SkillId,
                s.SkillName,
                s.Category,
                s.PeopleCount,
                s.AvgAllocatedPercent,
                s.AvgAvailablePercent,
                s.AvgAllocatedHours,
                s.AvgAvailableHours)).ToList());
    }
}
