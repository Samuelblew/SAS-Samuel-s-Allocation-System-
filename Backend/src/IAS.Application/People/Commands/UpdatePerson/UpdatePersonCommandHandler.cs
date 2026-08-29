using IAS.Application.Common.Exceptions;
using IAS.Application.People;
using MediatR;

namespace IAS.Application.People.Commands.UpdatePerson;

public sealed class UpdatePersonCommandHandler(IPersonRepository repository)
    : IRequestHandler<UpdatePersonCommand, PersonDto>
{
    public async Task<PersonDto> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
    {
        var person = await repository.GetByIdAsync(request.Id, includeSkills: true, cancellationToken)
            ?? throw new NotFoundException($"Pessoa '{request.Id}' não encontrada.");

        person.Name = request.Name.Trim();
        person.JobTitle = TrimOrNull(request.JobTitle);
        person.Seniority = TrimOrNull(request.Seniority);
        person.HourlyCost = request.HourlyCost;
        person.MonthlyCost = request.MonthlyCost;
        person.WeeklyCapacityHours = request.WeeklyCapacityHours;
        person.Location = TrimOrNull(request.Location);
        person.Team = TrimOrNull(request.Team);
        person.Status = request.Status;
        person.UpdatedAt = DateTime.UtcNow;

        await repository.SaveChangesAsync(cancellationToken);

        return person.ToDto();
    }

    private static string? TrimOrNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
