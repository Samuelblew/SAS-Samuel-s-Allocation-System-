using IAS.Application.Common.Interfaces;
using IAS.Application.People;
using IAS.Domain.People;
using MediatR;

namespace IAS.Application.People.Commands.CreatePerson;

public sealed class CreatePersonCommandHandler(
    IPersonRepository repository,
    ITenantContext tenantContext) : IRequestHandler<CreatePersonCommand, PersonDto>
{
    public async Task<PersonDto> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        if (!tenantContext.IsResolved)
            throw new InvalidOperationException("Tenant não resolvido.");

        var now = DateTime.UtcNow;
        var person = new Person
        {
            Id = Guid.NewGuid(),
            TenantId = tenantContext.TenantId,
            Name = request.Name.Trim(),
            JobTitle = TrimOrNull(request.JobTitle),
            Seniority = TrimOrNull(request.Seniority),
            HourlyCost = request.HourlyCost,
            MonthlyCost = request.MonthlyCost,
            WeeklyCapacityHours = request.WeeklyCapacityHours,
            Location = TrimOrNull(request.Location),
            Team = TrimOrNull(request.Team),
            Status = request.Status,
            CreatedAt = now
        };

        await repository.AddAsync(person, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return person.ToDto();
    }

    private static string? TrimOrNull(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
