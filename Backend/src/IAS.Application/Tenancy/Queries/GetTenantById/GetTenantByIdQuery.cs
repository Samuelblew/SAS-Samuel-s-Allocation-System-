using MediatR;

namespace IAS.Application.Tenancy.Queries.GetTenantById;

public sealed record GetTenantByIdQuery(Guid Id) : IRequest<TenantDto>;
