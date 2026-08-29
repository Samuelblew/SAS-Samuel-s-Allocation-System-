using IAS.Application.Allocations;
using MediatR;

namespace IAS.Application.Allocations.Queries.GetProjectPeopleView;

public sealed record GetProjectPeopleViewQuery(Guid ProjectId) : IRequest<ProjectPeopleViewDto>;
