using IAS.Application.Allocations;
using MediatR;

namespace IAS.Application.Allocations.Queries.GetPersonProjectsView;

public sealed record GetPersonProjectsViewQuery(Guid PersonId) : IRequest<PersonProjectsViewDto>;
