using IAS.Domain.People;
using MediatR;

namespace IAS.Application.People.Commands.UpdatePerson;

public sealed record UpdatePersonCommand(
    Guid Id,
    string Name,
    string? JobTitle,
    string? Seniority,
    decimal? HourlyCost,
    decimal? MonthlyCost,
    decimal WeeklyCapacityHours,
    string? Location,
    string? Team,
    PersonStatus Status) : IRequest<PersonDto>;
