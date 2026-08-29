using IAS.Domain.People;
using MediatR;

namespace IAS.Application.People.Commands.CreatePerson;

public sealed record CreatePersonCommand(
    string Name,
    string? JobTitle,
    string? Seniority,
    decimal? HourlyCost,
    decimal? MonthlyCost,
    decimal WeeklyCapacityHours,
    string? Location,
    string? Team,
    PersonStatus Status) : IRequest<PersonDto>;
