using MediatR;

namespace IAS.Application.Tenancy.Commands.CreateTenant;

public sealed record CreateTenantCommand(string Name) : IRequest<TenantDto>;
